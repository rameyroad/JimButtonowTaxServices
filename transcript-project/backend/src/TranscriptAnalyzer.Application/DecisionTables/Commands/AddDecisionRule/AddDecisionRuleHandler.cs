using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.AddDecisionRule;

public class AddDecisionRuleHandler : IRequestHandler<AddDecisionRuleCommand, DecisionRuleDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public AddDecisionRuleHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DecisionRuleDto?> Handle(
        AddDecisionRuleCommand request,
        CancellationToken cancellationToken)
    {
        var decisionTable = await _dbContext.Set<DecisionTable>()
            .FirstOrDefaultAsync(dt => dt.Id == request.DecisionTableId, cancellationToken);

        if (decisionTable is null)
            return null;

        var rule = new DecisionRule(decisionTable.Id, request.Priority, request.IsEnabled);

        foreach (var condition in request.Conditions)
        {
            rule.AddCondition(new RuleCondition(
                rule.Id,
                condition.ColumnKey,
                condition.Operator,
                condition.Value,
                condition.Value2));
        }

        foreach (var output in request.Outputs)
        {
            rule.AddOutput(new RuleOutput(
                rule.Id,
                output.ColumnKey,
                output.Value));
        }

        decisionTable.AddRule(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Reload the rule with all navigation properties
        var savedRule = await _dbContext.Set<DecisionRule>()
            .Include(r => r.Conditions)
            .Include(r => r.Outputs)
            .FirstAsync(r => r.Id == rule.Id, cancellationToken);

        return _mapper.Map<DecisionRuleDto>(savedRule);
    }
}
