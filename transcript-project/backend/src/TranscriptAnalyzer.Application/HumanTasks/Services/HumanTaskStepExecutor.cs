using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.HumanTasks.Services;

public class HumanTaskStepExecutor : IStepExecutor
{
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly DbContext _dbContext;

    public HumanTaskStepExecutor(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StepResult> ExecuteAsync(
        WorkflowStep workflowStep,
        StepExecution execution,
        CaseWorkflow caseWorkflow,
        CancellationToken cancellationToken = default)
    {
        // Parse configuration for task details
        var title = workflowStep.Name;
        string? description = null;
        Guid? assignedToUserId = null;
        DateTime? dueDate = null;

        if (!string.IsNullOrWhiteSpace(workflowStep.Configuration))
        {
            var config = JsonSerializer.Deserialize<HumanTaskStepConfig>(
                workflowStep.Configuration,
                CaseInsensitiveOptions);

            if (config is not null)
            {
                title = config.Title ?? title;
                description = config.Description;
                assignedToUserId = config.AssignedToUserId;

                if (config.DueDays > 0)
                {
                    dueDate = DateTime.UtcNow.AddDays(config.DueDays.Value);
                }
            }
        }

        // Create the human task entity
        var humanTask = new HumanTask(
            caseWorkflow.OrganizationId,
            caseWorkflow.Id,
            execution.Id,
            title,
            description,
            assignedToUserId,
            dueDate);

        _dbContext.Set<HumanTask>().Add(humanTask);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StepResult
        {
            Success = true,
            ShouldPause = true,
            OutputData = JsonSerializer.Serialize(new { humanTaskId = humanTask.Id })
        };
    }

#pragma warning disable CA1812 // Instantiated via JSON deserialization
    private sealed record HumanTaskStepConfig
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public Guid? AssignedToUserId { get; init; }
        public int? DueDays { get; init; }
    }
#pragma warning restore CA1812
}
