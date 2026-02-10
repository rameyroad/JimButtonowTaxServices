namespace TranscriptAnalyzer.Application.DecisionTables.DTOs;

public record DecisionRuleDto
{
    public required Guid Id { get; init; }
    public required int Priority { get; init; }
    public required bool IsEnabled { get; init; }
    public required IReadOnlyList<RuleConditionDto> Conditions { get; init; }
    public required IReadOnlyList<RuleOutputDto> Outputs { get; init; }
}
