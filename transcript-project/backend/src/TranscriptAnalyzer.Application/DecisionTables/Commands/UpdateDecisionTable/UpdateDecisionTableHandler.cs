using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionTable;

public class UpdateDecisionTableHandler : IRequestHandler<UpdateDecisionTableCommand, DecisionTableDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateDecisionTableHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DecisionTableDetailDto?> Handle(
        UpdateDecisionTableCommand request,
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

        if (request.Name is not null)
            decisionTable.UpdateName(request.Name);

        if (request.Description is not null)
            decisionTable.UpdateDescription(request.Description);

        if (request.Columns is not null)
        {
            // Replace all columns
            var existingColumns = decisionTable.Columns.ToList();
            foreach (var col in existingColumns)
                decisionTable.RemoveColumn(col);
            _dbContext.Set<DecisionTableColumn>().RemoveRange(existingColumns);

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
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DecisionTableDetailDto>(decisionTable);
    }
}
