using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Services;

public record StepResult
{
    public required bool Success { get; init; }
    public string? OutputData { get; init; }
    public string? ErrorMessage { get; init; }
    public bool ShouldPause { get; init; }
}

public interface IStepExecutor
{
    Task<StepResult> ExecuteAsync(
        WorkflowStep workflowStep,
        StepExecution execution,
        CaseWorkflow caseWorkflow,
        CancellationToken cancellationToken = default);
}
