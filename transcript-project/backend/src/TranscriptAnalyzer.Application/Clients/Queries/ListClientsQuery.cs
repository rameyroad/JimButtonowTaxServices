using MediatR;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Application.Common;

namespace TranscriptAnalyzer.Application.Clients.Queries;

public record ListClientsQuery : IRequest<PaginatedList<ClientListItemDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public string? ClientType { get; init; }
    public string SortBy { get; init; } = "name";
    public string SortOrder { get; init; } = "asc";
    public bool IncludeArchived { get; init; }
}
