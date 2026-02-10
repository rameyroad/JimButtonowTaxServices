using MediatR;
using TranscriptAnalyzer.Application.Formulas.DTOs;

namespace TranscriptAnalyzer.Application.Formulas.Commands.UnpublishFormula;

public record UnpublishFormulaCommand(Guid Id) : IRequest<FormulaDetailDto?>;
