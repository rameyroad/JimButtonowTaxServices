using MediatR;
using TranscriptAnalyzer.Application.Formulas.DTOs;

namespace TranscriptAnalyzer.Application.Formulas.Queries.GetFormula;

public record GetFormulaQuery(Guid Id) : IRequest<FormulaDetailDto?>;
