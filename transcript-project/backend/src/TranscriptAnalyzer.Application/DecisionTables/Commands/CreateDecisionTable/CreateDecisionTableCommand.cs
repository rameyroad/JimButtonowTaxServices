using MediatR;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.CreateDecisionTable;

public record CreateDecisionTableCommand : IRequest<DecisionTableDetailDto>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required IReadOnlyList<ColumnDefinition> Columns { get; init; }
}

public record ColumnDefinition
{
    public required string Name { get; init; }
    public required string Key { get; init; }
    public required DataType DataType { get; init; }
    public required bool IsInput { get; init; }
    public int SortOrder { get; init; }
}
