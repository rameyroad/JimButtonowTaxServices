using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class RuleCondition : BaseEntity
{
    public Guid DecisionRuleId { get; private set; }
    public string ColumnKey { get; private set; }
    public ConditionOperator Operator { get; private set; }
    public string Value { get; private set; }
    public string? Value2 { get; private set; }

    public DecisionRule DecisionRule { get; private set; } = null!;

#pragma warning disable CS8618 // Required for EF Core
    private RuleCondition() { }
#pragma warning restore CS8618

    public RuleCondition(
        Guid decisionRuleId,
        string columnKey,
        ConditionOperator @operator,
        string value,
        string? value2 = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnKey);
        ArgumentNullException.ThrowIfNull(value);

        DecisionRuleId = decisionRuleId;
        ColumnKey = columnKey.Trim();
        Operator = @operator;
        Value = value;
        Value2 = value2;
    }
}
