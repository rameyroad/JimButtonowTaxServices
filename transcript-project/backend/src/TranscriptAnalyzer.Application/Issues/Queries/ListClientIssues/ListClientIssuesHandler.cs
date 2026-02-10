using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Issues.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Issues.Queries.ListClientIssues;

public class ListClientIssuesHandler : IRequestHandler<ListClientIssuesQuery, PaginatedList<IssueListItemDto>>
{
    private readonly DbContext _dbContext;

    public ListClientIssuesHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<IssueListItemDto>> Handle(
        ListClientIssuesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<Issue>()
            .Where(i => i.ClientId == request.ClientId);

        // Filter by issue type
        if (request.IssueType.HasValue)
        {
            query = query.Where(i => i.IssueType == request.IssueType.Value);
        }

        // Filter by severity
        if (request.Severity.HasValue)
        {
            query = query.Where(i => i.Severity == request.Severity.Value);
        }

        // Filter by tax year
        if (request.TaxYear.HasValue)
        {
            query = query.Where(i => i.TaxYear == request.TaxYear.Value);
        }

        // Filter by resolved status
        if (request.Resolved.HasValue)
        {
            query = request.Resolved.Value
                ? query.Where(i => i.ResolvedAt != null)
                : query.Where(i => i.ResolvedAt == null);
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        var dtoQuery = query.Select(i => new IssueListItemDto
        {
            Id = i.Id,
            ClientId = i.ClientId,
            IssueType = i.IssueType,
            Severity = i.Severity,
            TaxYear = i.TaxYear,
            Amount = i.Amount,
            Description = i.Description,
            TransactionCode = i.TransactionCode,
            DetectedAt = i.DetectedAt,
            ResolvedAt = i.ResolvedAt,
            CreatedAt = i.CreatedAt
        });

        return await PaginatedList<IssueListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<Issue> ApplySorting(
        IQueryable<Issue> query, string sortBy, string sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "SEVERITY" => isDescending
                ? query.OrderByDescending(i => i.Severity)
                : query.OrderBy(i => i.Severity),
            "TAXYEAR" => isDescending
                ? query.OrderByDescending(i => i.TaxYear)
                : query.OrderBy(i => i.TaxYear),
            "AMOUNT" => isDescending
                ? query.OrderByDescending(i => i.Amount)
                : query.OrderBy(i => i.Amount),
            "ISSUETYPE" => isDescending
                ? query.OrderByDescending(i => i.IssueType)
                : query.OrderBy(i => i.IssueType),
            _ => isDescending
                ? query.OrderByDescending(i => i.DetectedAt)
                : query.OrderBy(i => i.DetectedAt)
        };
    }
}
