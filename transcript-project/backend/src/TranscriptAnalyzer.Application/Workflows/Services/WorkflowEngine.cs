using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Services;

public class WorkflowEngine : IWorkflowEngine
{
    private readonly DbContext _dbContext;
    private readonly Dictionary<WorkflowStepType, IStepExecutor> _executors;
    private readonly ILogger<WorkflowEngine> _logger;

    public WorkflowEngine(
        DbContext dbContext,
        IEnumerable<KeyValuePair<WorkflowStepType, IStepExecutor>> executors,
        ILogger<WorkflowEngine> logger)
    {
        _dbContext = dbContext;
        _executors = executors.ToDictionary(kv => kv.Key, kv => kv.Value);
        _logger = logger;
    }

    public async Task StartWorkflowAsync(CaseWorkflow caseWorkflow, CancellationToken cancellationToken = default)
    {
        // Load the workflow definition with steps
        var definition = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstOrDefaultAsync(wd => wd.Id == caseWorkflow.WorkflowDefinitionId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow definition {caseWorkflow.WorkflowDefinitionId} not found.");

        var orderedSteps = definition.Steps.OrderBy(s => s.SortOrder).ToList();
        if (orderedSteps.Count == 0)
        {
            caseWorkflow.Complete();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var firstStep = orderedSteps[0];
        caseWorkflow.Start(firstStep.Id);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await ExecuteStepsAsync(caseWorkflow, orderedSteps, 0, cancellationToken);
    }

    public async Task ResumeWorkflowAsync(CaseWorkflow caseWorkflow, CancellationToken cancellationToken = default)
    {
        if (caseWorkflow.CurrentStepId is null)
        {
            caseWorkflow.Complete();
            await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var definition = await _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .FirstOrDefaultAsync(wd => wd.Id == caseWorkflow.WorkflowDefinitionId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow definition {caseWorkflow.WorkflowDefinitionId} not found.");

        var orderedSteps = definition.Steps.OrderBy(s => s.SortOrder).ToList();
        var currentIndex = orderedSteps.FindIndex(s => s.Id == caseWorkflow.CurrentStepId);

        if (currentIndex < 0)
        {
            caseWorkflow.Fail("Current step not found in workflow definition.");
            await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        caseWorkflow.Resume(caseWorkflow.CurrentStepId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await ExecuteStepsAsync(caseWorkflow, orderedSteps, currentIndex, cancellationToken);
    }

    private async Task ExecuteStepsAsync(
        CaseWorkflow caseWorkflow,
        List<WorkflowStep> orderedSteps,
        int startIndex,
        CancellationToken cancellationToken)
    {
        // Collect output from previous steps to pass as context
        var workflowContext = await BuildWorkflowContextAsync(caseWorkflow, cancellationToken);

        for (var i = startIndex; i < orderedSteps.Count; i++)
        {
            var step = orderedSteps[i];

            if (!_executors.TryGetValue(step.StepType, out var executor))
            {
                _logger.LogWarning(
                    "No executor registered for step type {StepType}, skipping step {StepName}",
                    step.StepType, step.Name);

                if (step.IsRequired)
                {
                    caseWorkflow.Fail($"No executor available for required step type: {step.StepType}");
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return;
                }

                continue;
            }

            // Create step execution
            var stepExecution = new StepExecution(
                caseWorkflow.OrganizationId,
                caseWorkflow.Id,
                step.Id);
            stepExecution.Start(JsonSerializer.Serialize(workflowContext));

            caseWorkflow.AddStepExecution(stepExecution);
            caseWorkflow.MoveToStep(step.Id);
            await _dbContext.SaveChangesAsync(cancellationToken);

            try
            {
                var result = await executor.ExecuteAsync(step, stepExecution, caseWorkflow, cancellationToken);

                if (result.ShouldPause)
                {
                    stepExecution.Complete(result.OutputData);
                    caseWorkflow.Pause();
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return;
                }

                if (result.Success)
                {
                    stepExecution.Complete(result.OutputData);

                    // Merge step output into workflow context
                    if (result.OutputData is not null)
                    {
                        workflowContext[step.Name] = result.OutputData;
                    }

                    // Check for explicit next step routing
                    if (step.NextStepOnSuccessId.HasValue)
                    {
                        var nextIndex = orderedSteps.FindIndex(s => s.Id == step.NextStepOnSuccessId.Value);
                        if (nextIndex >= 0)
                        {
                            i = nextIndex - 1; // will be incremented by loop
                        }
                    }
                }
                else
                {
                    stepExecution.Fail(result.ErrorMessage ?? "Step execution failed.");

                    if (step.NextStepOnFailureId.HasValue)
                    {
                        var failIndex = orderedSteps.FindIndex(s => s.Id == step.NextStepOnFailureId.Value);
                        if (failIndex >= 0)
                        {
                            i = failIndex - 1; // will be incremented by loop
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            continue;
                        }
                    }

                    if (step.IsRequired)
                    {
                        caseWorkflow.Fail(result.ErrorMessage ?? "Required step failed.");
                        await _dbContext.SaveChangesAsync(cancellationToken);
                        return;
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error executing step {StepName} in workflow {WorkflowId}",
                    step.Name, caseWorkflow.Id);

                stepExecution.Fail(ex.Message);

                if (step.IsRequired)
                {
                    caseWorkflow.Fail($"Step '{step.Name}' threw an exception: {ex.Message}");
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        // All steps completed
        caseWorkflow.Complete();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<string, string>> BuildWorkflowContextAsync(
        CaseWorkflow caseWorkflow,
        CancellationToken cancellationToken)
    {
        var context = new Dictionary<string, string>();

        var previousExecutions = await _dbContext.Set<StepExecution>()
            .Include(se => se.WorkflowStep)
            .Where(se => se.CaseWorkflowId == caseWorkflow.Id && se.Status == StepExecutionStatus.Completed)
            .OrderBy(se => se.CompletedAt)
            .ToListAsync(cancellationToken);

        foreach (var exec in previousExecutions)
        {
            if (exec.OutputData is not null)
            {
                context[exec.WorkflowStep.Name] = exec.OutputData;
            }
        }

        return context;
    }
}
