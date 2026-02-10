using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Formulas.Commands.CreateFormula;

public class CreateFormulaHandler : IRequestHandler<CreateFormulaCommand, FormulaDetailDto>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateFormulaHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<FormulaDetailDto> Handle(
        CreateFormulaCommand request,
        CancellationToken cancellationToken)
    {
        var formula = new CalculationFormula(
            request.Name,
            request.Expression,
            request.InputVariables,
            request.OutputType,
            request.Description);

        _dbContext.Set<CalculationFormula>().Add(formula);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FormulaDetailDto>(formula);
    }
}
