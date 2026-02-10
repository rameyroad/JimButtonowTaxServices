using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.RemoveDecisionRule;

public class RemoveDecisionRuleHandler : IRequestHandler<RemoveDecisionRuleCommand, bool>
{
    private readonly DbContext _dbContext;

    public RemoveDecisionRuleHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        RemoveDecisionRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = await _dbContext.Set<DecisionRule>()
            .FirstOrDefaultAsync(r => r.Id == request.RuleId && r.DecisionTableId == request.DecisionTableId,
                cancellationToken);

        if (rule is null)
            return false;

        var decisionTable = await _dbContext.Set<DecisionTable>()
            .Include(dt => dt.Rules)
            .FirstOrDefaultAsync(dt => dt.Id == request.DecisionTableId, cancellationToken);

        if (decisionTable is null)
            return false;

        decisionTable.RemoveRule(rule);
        _dbContext.Set<DecisionRule>().Remove(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
