using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.HumanTasks.DTOs;

public record HumanTaskListItemDto
{
    public required Guid Id { get; init; }
    public required Guid CaseWorkflowId { get; init; }
    public required Guid StepExecutionId { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public required string Title { get; init; }
    public DateTime? DueDate { get; init; }
    public required HumanTaskStatus Status { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? Decision { get; init; }
    public required DateTime CreatedAt { get; init; }
}
