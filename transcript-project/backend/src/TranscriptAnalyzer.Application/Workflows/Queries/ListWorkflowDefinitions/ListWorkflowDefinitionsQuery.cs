using MediatR;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Queries.ListWorkflowDefinitions;

public record ListWorkflowDefinitionsQuery : IRequest<PaginatedList<WorkflowDefinitionListItemDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
    public PublishStatus? Status { get; init; }
    public string? Category { get; init; }
    public string SortBy { get; init; } = "name";
    public string SortOrder { get; init; } = "asc";
    public bool PublishedOnly { get; init; }
}
