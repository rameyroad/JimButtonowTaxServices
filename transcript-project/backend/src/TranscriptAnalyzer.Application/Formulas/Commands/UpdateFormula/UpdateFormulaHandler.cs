using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Formulas.Commands.UpdateFormula;

public class UpdateFormulaHandler : IRequestHandler<UpdateFormulaCommand, FormulaDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateFormulaHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<FormulaDetailDto?> Handle(
        UpdateFormulaCommand request,
        CancellationToken cancellationToken)
    {
        var formula = await _dbContext.Set<CalculationFormula>()
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (formula is null)
            return null;

        if (request.Name is not null)
            formula.UpdateName(request.Name);

        if (request.Description is not null)
            formula.UpdateDescription(request.Description);

        if (request.Expression is not null)
            formula.UpdateExpression(request.Expression);

        if (request.InputVariables is not null)
            formula.UpdateInputVariables(request.InputVariables);

        if (request.OutputType.HasValue)
            formula.UpdateOutputType(request.OutputType.Value);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FormulaDetailDto>(formula);
    }
}
