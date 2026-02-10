using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Formulas.Queries.GetFormula;

public class GetFormulaHandler : IRequestHandler<GetFormulaQuery, FormulaDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public GetFormulaHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<FormulaDetailDto?> Handle(
        GetFormulaQuery request,
        CancellationToken cancellationToken)
    {
        var formula = await _dbContext.Set<CalculationFormula>()
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (formula is null)
            return null;

        return _mapper.Map<FormulaDetailDto>(formula);
    }
}
