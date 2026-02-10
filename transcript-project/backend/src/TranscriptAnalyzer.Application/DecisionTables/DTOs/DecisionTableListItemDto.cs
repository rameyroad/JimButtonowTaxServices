using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.DecisionTables.DTOs;

public record DecisionTableListItemDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required PublishStatus Status { get; init; }
    public int RuleCount { get; init; }
    public int ColumnCount { get; init; }
    public DateTime? PublishedAt { get; init; }
    public required int Version { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
