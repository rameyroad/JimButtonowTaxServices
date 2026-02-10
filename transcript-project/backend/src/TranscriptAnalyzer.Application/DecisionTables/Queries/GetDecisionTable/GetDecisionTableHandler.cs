using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Queries.GetDecisionTable;

public class GetDecisionTableHandler : IRequestHandler<GetDecisionTableQuery, DecisionTableDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public GetDecisionTableHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DecisionTableDetailDto?> Handle(
        GetDecisionTableQuery request,
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

        return _mapper.Map<DecisionTableDetailDto>(decisionTable);
    }
}
