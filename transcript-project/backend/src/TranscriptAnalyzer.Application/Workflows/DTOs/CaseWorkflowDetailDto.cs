using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.DTOs;

public record CaseWorkflowDetailDto
{
    public required Guid Id { get; init; }
    public required Guid ClientId { get; init; }
    public required Guid WorkflowDefinitionId { get; init; }
    public required string WorkflowName { get; init; }
    public required int WorkflowVersion { get; init; }
    public required WorkflowExecutionStatus Status { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public Guid StartedByUserId { get; init; }
    public Guid? CurrentStepId { get; init; }
    public string? ErrorMessage { get; init; }
    public required IReadOnlyList<StepExecutionDto> StepExecutions { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
