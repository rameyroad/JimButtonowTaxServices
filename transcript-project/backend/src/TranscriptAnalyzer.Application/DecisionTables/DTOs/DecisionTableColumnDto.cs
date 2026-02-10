using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.DecisionTables.DTOs;

public record DecisionTableColumnDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Key { get; init; }
    public required DataType DataType { get; init; }
    public required bool IsInput { get; init; }
    public required int SortOrder { get; init; }
}
