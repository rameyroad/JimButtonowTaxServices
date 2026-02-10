using MediatR;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.DecisionTables.Queries.ListDecisionTables;

public record ListDecisionTablesQuery : IRequest<PaginatedList<DecisionTableListItemDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public PublishStatus? Status { get; init; }
    public string SortBy { get; init; } = "name";
    public string SortOrder { get; init; } = "asc";
}
