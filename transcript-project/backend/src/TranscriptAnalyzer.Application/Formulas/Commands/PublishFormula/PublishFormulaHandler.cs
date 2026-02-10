using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Formulas.Commands.PublishFormula;

public class PublishFormulaHandler : IRequestHandler<PublishFormulaCommand, FormulaDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public PublishFormulaHandler(DbContext dbContext, ITenantContext tenantContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<FormulaDetailDto?> Handle(
        PublishFormulaCommand request,
        CancellationToken cancellationToken)
    {
        var formula = await _dbContext.Set<CalculationFormula>()
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (formula is null)
            return null;

        var userId = _tenantContext.UserId
            ?? throw new UnauthorizedAccessException("User context is required.");

        formula.Publish(userId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FormulaDetailDto>(formula);
    }
}
