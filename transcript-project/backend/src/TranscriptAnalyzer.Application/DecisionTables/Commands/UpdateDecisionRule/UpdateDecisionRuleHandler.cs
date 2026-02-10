using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionRule;

public class UpdateDecisionRuleHandler : IRequestHandler<UpdateDecisionRuleCommand, DecisionRuleDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateDecisionRuleHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DecisionRuleDto?> Handle(
        UpdateDecisionRuleCommand request,
        CancellationToken cancellationToken)
    {
        var rule = await _dbContext.Set<DecisionRule>()
            .Include(r => r.Conditions)
            .Include(r => r.Outputs)
            .FirstOrDefaultAsync(r => r.Id == request.RuleId && r.DecisionTableId == request.DecisionTableId,
                cancellationToken);

        if (rule is null)
            return null;

        if (request.Priority.HasValue)
            rule.UpdatePriority(request.Priority.Value);

        if (request.IsEnabled.HasValue)
            rule.SetEnabled(request.IsEnabled.Value);

        if (request.Conditions is not null)
        {
            _dbContext.Set<RuleCondition>().RemoveRange(rule.Conditions);
            rule.ClearConditions();

            foreach (var condition in request.Conditions)
            {
                rule.AddCondition(new RuleCondition(
                    rule.Id,
                    condition.ColumnKey,
                    condition.Operator,
                    condition.Value,
                    condition.Value2));
            }
        }

        if (request.Outputs is not null)
        {
            _dbContext.Set<RuleOutput>().RemoveRange(rule.Outputs);
            rule.ClearOutputs();

            foreach (var output in request.Outputs)
            {
                rule.AddOutput(new RuleOutput(
                    rule.Id,
                    output.ColumnKey,
                    output.Value));
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Reload with fresh data
        var savedRule = await _dbContext.Set<DecisionRule>()
            .Include(r => r.Conditions)
            .Include(r => r.Outputs)
            .FirstAsync(r => r.Id == rule.Id, cancellationToken);

        return _mapper.Map<DecisionRuleDto>(savedRule);
    }
}
