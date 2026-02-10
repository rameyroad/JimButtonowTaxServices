using TranscriptAnalyzer.Domain.Common;

namespace TranscriptAnalyzer.Domain.Entities;

public class RuleOutput : BaseEntity
{
    public Guid DecisionRuleId { get; private set; }
    public string ColumnKey { get; private set; }
    public string Value { get; private set; }

    public DecisionRule DecisionRule { get; private set; } = null!;

#pragma warning disable CS8618 // Required for EF Core
    private RuleOutput() { }
#pragma warning restore CS8618

    public RuleOutput(Guid decisionRuleId, string columnKey, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(columnKey);
        ArgumentNullException.ThrowIfNull(value);

        DecisionRuleId = decisionRuleId;
        ColumnKey = columnKey.Trim();
        Value = value;
    }
}
