namespace TranscriptAnalyzer.Application.DecisionTables.DTOs;

public record RuleOutputDto
{
    public required Guid Id { get; init; }
    public required string ColumnKey { get; init; }
    public required string Value { get; init; }
}
