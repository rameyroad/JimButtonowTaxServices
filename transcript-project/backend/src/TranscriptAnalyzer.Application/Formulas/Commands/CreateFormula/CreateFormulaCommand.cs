using MediatR;
using TranscriptAnalyzer.Application.Formulas.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Formulas.Commands.CreateFormula;

public record CreateFormulaCommand : IRequest<FormulaDetailDto>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Expression { get; init; }
    public required string InputVariables { get; init; }
    public required DataType OutputType { get; init; }
}
