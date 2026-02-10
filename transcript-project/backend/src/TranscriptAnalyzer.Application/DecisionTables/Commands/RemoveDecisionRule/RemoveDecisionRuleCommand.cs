using MediatR;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.RemoveDecisionRule;

public record RemoveDecisionRuleCommand(Guid DecisionTableId, Guid RuleId) : IRequest<bool>;
