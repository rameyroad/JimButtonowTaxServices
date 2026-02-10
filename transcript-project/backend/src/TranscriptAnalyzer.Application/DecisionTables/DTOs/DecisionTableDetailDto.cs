using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.DecisionTables.DTOs;

public record DecisionTableDetailDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required PublishStatus Status { get; init; }
    public DateTime? PublishedAt { get; init; }
    public Guid? PublishedByUserId { get; init; }
    public required int Version { get; init; }
    public required IReadOnlyList<DecisionTableColumnDto> Columns { get; init; }
    public required IReadOnlyList<DecisionRuleDto> Rules { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
