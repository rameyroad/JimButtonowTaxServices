using MediatR;
using TranscriptAnalyzer.Application.DecisionTables.Commands.CreateDecisionTable;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionTable;

public record UpdateDecisionTableCommand : IRequest<DecisionTableDetailDto?>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public IReadOnlyList<ColumnDefinition>? Columns { get; init; }
}
