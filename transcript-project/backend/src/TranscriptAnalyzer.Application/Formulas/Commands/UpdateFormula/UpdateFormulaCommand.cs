using MediatR;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Formulas.Commands.UpdateFormula;

public record UpdateFormulaCommand : IRequest<FormulaDetailDto?>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Expression { get; init; }
    public string? InputVariables { get; init; }
    public DataType? OutputType { get; init; }
}
