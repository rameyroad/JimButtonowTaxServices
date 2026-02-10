using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.CreateDecisionTable;

public class CreateDecisionTableHandler : IRequestHandler<CreateDecisionTableCommand, DecisionTableDetailDto>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateDecisionTableHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DecisionTableDetailDto> Handle(
        CreateDecisionTableCommand request,
        CancellationToken cancellationToken)
    {
        var decisionTable = new DecisionTable(request.Name, request.Description);

        for (var i = 0; i < request.Columns.Count; i++)
        {
            var col = request.Columns[i];
            var column = new DecisionTableColumn(
                decisionTable.Id,
                col.Name,
                col.Key,
                col.DataType,
                col.IsInput,
                col.SortOrder > 0 ? col.SortOrder : i + 1);
            decisionTable.AddColumn(column);
        }

        _dbContext.Set<DecisionTable>().Add(decisionTable);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Reload with all navigation properties
        var result = await _dbContext.Set<DecisionTable>()
            .Include(dt => dt.Columns)
            .Include(dt => dt.Rules)
                .ThenInclude(r => r.Conditions)
            .Include(dt => dt.Rules)
                .ThenInclude(r => r.Outputs)
            .FirstAsync(dt => dt.Id == decisionTable.Id, cancellationToken);

        return _mapper.Map<DecisionTableDetailDto>(result);
    }
}
