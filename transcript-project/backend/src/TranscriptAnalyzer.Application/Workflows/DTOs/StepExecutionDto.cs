using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.DTOs;

public record StepExecutionDto
{
    public required Guid Id { get; init; }
    public required Guid WorkflowStepId { get; init; }
    public required string StepName { get; init; }
    public required WorkflowStepType StepType { get; init; }
    public required StepExecutionStatus Status { get; init; }
    public string? InputData { get; init; }
    public string? OutputData { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
}
