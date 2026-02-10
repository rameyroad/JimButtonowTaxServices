using TranscriptAnalyzer.Domain.Common;

namespace TranscriptAnalyzer.Domain.Entities;

public class DecisionRule : BaseEntity
{
    public Guid DecisionTableId { get; private set; }
    public int Priority { get; private set; }
    public bool IsEnabled { get; private set; }

    private readonly List<RuleCondition> _conditions = [];
    public IReadOnlyCollection<RuleCondition> Conditions => _conditions.AsReadOnly();

    private readonly List<RuleOutput> _outputs = [];
    public IReadOnlyCollection<RuleOutput> Outputs => _outputs.AsReadOnly();

    public DecisionTable DecisionTable { get; private set; } = null!;

    private DecisionRule() { }

    public DecisionRule(Guid decisionTableId, int priority, bool isEnabled = true)
    {
        DecisionTableId = decisionTableId;
        Priority = priority;
        IsEnabled = isEnabled;
    }

    public void UpdatePriority(int priority)
    {
        Priority = priority;
        SetUpdatedAt();
    }

    public void SetEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
        SetUpdatedAt();
    }

    public void AddCondition(RuleCondition condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        _conditions.Add(condition);
        SetUpdatedAt();
    }

    public void ClearConditions()
    {
        _conditions.Clear();
        SetUpdatedAt();
    }

    public void AddOutput(RuleOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _outputs.Add(output);
        SetUpdatedAt();
    }

    public void ClearOutputs()
    {
        _outputs.Clear();
        SetUpdatedAt();
    }
}
