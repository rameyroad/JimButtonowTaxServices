using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.DTOs;

public record WorkflowDefinitionListItemDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
    public required PublishStatus Status { get; init; }
    public int StepCount { get; init; }
    public required int CurrentVersion { get; init; }
    public DateTime? PublishedAt { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
