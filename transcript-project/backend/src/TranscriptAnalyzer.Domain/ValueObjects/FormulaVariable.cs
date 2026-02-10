using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.ValueObjects;

public record FormulaVariable
{
    public required string Name { get; init; }
    public required DataType DataType { get; init; }
    public string? Description { get; init; }
}
