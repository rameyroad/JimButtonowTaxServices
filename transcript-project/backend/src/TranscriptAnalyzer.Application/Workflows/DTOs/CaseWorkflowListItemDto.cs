using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.DTOs;

public record CaseWorkflowListItemDto
{
    public required Guid Id { get; init; }
    public required Guid ClientId { get; init; }
    public required string WorkflowName { get; init; }
    public required int WorkflowVersion { get; init; }
    public required WorkflowExecutionStatus Status { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public int TotalSteps { get; init; }
    public int CompletedSteps { get; init; }
    public required DateTime CreatedAt { get; init; }
}
