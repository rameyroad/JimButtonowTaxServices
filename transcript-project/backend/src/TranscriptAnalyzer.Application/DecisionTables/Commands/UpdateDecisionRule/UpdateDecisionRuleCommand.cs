using MediatR;
using TranscriptAnalyzer.Application.DecisionTables.Commands.AddDecisionRule;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionRule;

public record UpdateDecisionRuleCommand : IRequest<DecisionRuleDto?>
{
    public Guid DecisionTableId { get; init; }
    public Guid RuleId { get; init; }
    public int? Priority { get; init; }
    public bool? IsEnabled { get; init; }
    public IReadOnlyList<RuleConditionInput>? Conditions { get; init; }
    public IReadOnlyList<RuleOutputInput>? Outputs { get; init; }
}
