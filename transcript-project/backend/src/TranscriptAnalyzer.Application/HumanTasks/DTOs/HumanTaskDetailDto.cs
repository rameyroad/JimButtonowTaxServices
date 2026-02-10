using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.HumanTasks.DTOs;

public record HumanTaskDetailDto
{
    public required Guid Id { get; init; }
    public required Guid CaseWorkflowId { get; init; }
    public required Guid StepExecutionId { get; init; }
    public Guid? AssignedToUserId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTime? DueDate { get; init; }
    public required HumanTaskStatus Status { get; init; }
    public DateTime? CompletedAt { get; init; }
    public Guid? CompletedByUserId { get; init; }
    public string? Decision { get; init; }
    public string? Notes { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
