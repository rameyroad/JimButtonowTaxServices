using MediatR;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.AddDecisionRule;

public record AddDecisionRuleCommand : IRequest<DecisionRuleDto?>
{
    public Guid DecisionTableId { get; init; }
    public int Priority { get; init; }
    public bool IsEnabled { get; init; } = true;
    public required IReadOnlyList<RuleConditionInput> Conditions { get; init; }
    public required IReadOnlyList<RuleOutputInput> Outputs { get; init; }
}

public record RuleConditionInput
{
    public required string ColumnKey { get; init; }
    public required ConditionOperator Operator { get; init; }
    public required string Value { get; init; }
    public string? Value2 { get; init; }
}

public record RuleOutputInput
{
    public required string ColumnKey { get; init; }
    public required string Value { get; init; }
}
