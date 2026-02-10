using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Formulas.Commands.UnpublishFormula;

public class UnpublishFormulaHandler : IRequestHandler<UnpublishFormulaCommand, FormulaDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UnpublishFormulaHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<FormulaDetailDto?> Handle(
        UnpublishFormulaCommand request,
        CancellationToken cancellationToken)
    {
        var formula = await _dbContext.Set<CalculationFormula>()
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (formula is null)
            return null;

        formula.Unpublish();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FormulaDetailDto>(formula);
    }
}
