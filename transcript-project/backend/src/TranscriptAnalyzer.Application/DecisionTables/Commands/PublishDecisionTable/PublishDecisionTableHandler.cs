using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.PublishDecisionTable;

public class PublishDecisionTableHandler : IRequestHandler<PublishDecisionTableCommand, DecisionTableDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public PublishDecisionTableHandler(
        DbContext dbContext,
        ITenantContext tenantContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<DecisionTableDetailDto?> Handle(
        PublishDecisionTableCommand request,
        CancellationToken cancellationToken)
    {
        var decisionTable = await _dbContext.Set<DecisionTable>()
            .Include(dt => dt.Columns)
            .Include(dt => dt.Rules)
                .ThenInclude(r => r.Conditions)
            .Include(dt => dt.Rules)
                .ThenInclude(r => r.Outputs)
            .FirstOrDefaultAsync(dt => dt.Id == request.Id, cancellationToken);

        if (decisionTable is null)
            return null;

        if (decisionTable.Rules.Count == 0)
            throw new InvalidOperationException("Cannot publish a decision table with no rules.");

        var userId = _tenantContext.UserId
            ?? throw new UnauthorizedAccessException("User context is required.");

        decisionTable.Publish(userId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DecisionTableDetailDto>(decisionTable);
    }
}
