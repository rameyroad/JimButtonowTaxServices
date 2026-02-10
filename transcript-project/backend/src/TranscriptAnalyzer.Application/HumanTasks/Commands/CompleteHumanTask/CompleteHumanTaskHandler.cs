using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.HumanTasks.Commands.CompleteHumanTask;

public class CompleteHumanTaskHandler : IRequestHandler<CompleteHumanTaskCommand, HumanTaskDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IWorkflowEngine _workflowEngine;
    private readonly IMapper _mapper;

    public CompleteHumanTaskHandler(
        DbContext dbContext,
        ITenantContext tenantContext,
        IWorkflowEngine workflowEngine,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _workflowEngine = workflowEngine;
        _mapper = mapper;
    }

    public async Task<HumanTaskDetailDto?> Handle(
        CompleteHumanTaskCommand request,
        CancellationToken cancellationToken)
    {
        var humanTask = await _dbContext.Set<HumanTask>()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (humanTask is null)
            return null;

        if (humanTask.Status == HumanTaskStatus.Completed)
            return _mapper.Map<HumanTaskDetailDto>(humanTask);

        // Complete the human task
        humanTask.Complete(
            _tenantContext.UserId ?? Guid.Empty,
            request.Decision,
            request.Notes);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Resume the workflow — advance to next step
        var caseWorkflow = await _dbContext.Set<CaseWorkflow>()
            .FirstOrDefaultAsync(cw => cw.Id == humanTask.CaseWorkflowId, cancellationToken);

        if (caseWorkflow is not null && caseWorkflow.Status == WorkflowExecutionStatus.Paused)
        {
            // Find the next step after the current one
            var definition = await _dbContext.Set<WorkflowDefinition>()
                .Include(wd => wd.Steps)
                .FirstOrDefaultAsync(wd => wd.Id == caseWorkflow.WorkflowDefinitionId, cancellationToken);

            if (definition is not null && caseWorkflow.CurrentStepId.HasValue)
            {
                var orderedSteps = definition.Steps.OrderBy(s => s.SortOrder).ToList();
                var currentStep = orderedSteps.FirstOrDefault(s => s.Id == caseWorkflow.CurrentStepId.Value);
                var currentIndex = orderedSteps.IndexOf(currentStep!);

                // Determine next step
                Guid? nextStepId = null;
                if (currentStep?.NextStepOnSuccessId.HasValue == true)
                {
                    nextStepId = currentStep.NextStepOnSuccessId;
                }
                else if (currentIndex + 1 < orderedSteps.Count)
                {
                    nextStepId = orderedSteps[currentIndex + 1].Id;
                }

                caseWorkflow.MoveToStep(nextStepId);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await _workflowEngine.ResumeWorkflowAsync(caseWorkflow, cancellationToken);
            }
        }

        return _mapper.Map<HumanTaskDetailDto>(humanTask);
    }
}
