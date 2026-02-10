using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.DecisionTables.DTOs;

public record RuleConditionDto
{
    public required Guid Id { get; init; }
    public required string ColumnKey { get; init; }
    public required ConditionOperator Operator { get; init; }
    public required string Value { get; init; }
    public string? Value2 { get; init; }
}
