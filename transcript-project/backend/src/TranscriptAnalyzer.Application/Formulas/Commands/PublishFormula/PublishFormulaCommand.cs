using MediatR;
using TranscriptAnalyzer.Application.Formulas.DTOs;

namespace TranscriptAnalyzer.Application.Formulas.Commands.PublishFormula;

public record PublishFormulaCommand(Guid Id) : IRequest<FormulaDetailDto?>;
