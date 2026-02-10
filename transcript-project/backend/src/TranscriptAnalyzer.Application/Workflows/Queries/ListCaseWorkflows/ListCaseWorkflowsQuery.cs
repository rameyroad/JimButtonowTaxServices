using MediatR;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Queries.ListCaseWorkflows;

public record ListCaseWorkflowsQuery : IRequest<PaginatedList<CaseWorkflowListItemDto>>
{
    public required Guid ClientId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public WorkflowExecutionStatus? Status { get; init; }
    public string SortBy { get; init; } = "createdAt";
    public string SortOrder { get; init; } = "desc";
}
