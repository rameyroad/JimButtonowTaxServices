using MediatR;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Issues.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Issues.Queries.ListClientIssues;

public record ListClientIssuesQuery : IRequest<PaginatedList<IssueListItemDto>>
{
    public required Guid ClientId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public IssueType? IssueType { get; init; }
    public IssueSeverity? Severity { get; init; }
    public int? TaxYear { get; init; }
    public bool? Resolved { get; init; }
    public string SortBy { get; init; } = "detectedAt";
    public string SortOrder { get; init; } = "desc";
}
