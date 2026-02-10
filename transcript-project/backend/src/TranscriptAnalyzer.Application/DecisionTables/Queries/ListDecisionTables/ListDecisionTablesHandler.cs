using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.DecisionTables.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.DecisionTables.Queries.ListDecisionTables;

public class ListDecisionTablesHandler : IRequestHandler<ListDecisionTablesQuery, PaginatedList<DecisionTableListItemDto>>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public ListDecisionTablesHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedList<DecisionTableListItemDto>> Handle(
        ListDecisionTablesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<DecisionTable>()
            .Include(dt => dt.Rules)
            .Include(dt => dt.Columns)
            .AsQueryable();

        // Filter by status
        if (request.Status.HasValue)
        {
            query = query.Where(dt => dt.Status == request.Status.Value);
        }

        // Search by name or description
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search.Trim()}%";
#pragma warning disable CA1307, CA1304
            query = query.Where(dt =>
                EF.Functions.Like(dt.Name, searchPattern) ||
                EF.Functions.Like(dt.Description ?? string.Empty, searchPattern));
#pragma warning restore CA1307, CA1304
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        var dtoQuery = query.Select(dt => new DecisionTableListItemDto
        {
            Id = dt.Id,
            Name = dt.Name,
            Description = dt.Description,
            Status = dt.Status,
            RuleCount = dt.Rules.Count,
            ColumnCount = dt.Columns.Count,
            PublishedAt = dt.PublishedAt,
            Version = dt.Version,
            CreatedAt = dt.CreatedAt,
            UpdatedAt = dt.UpdatedAt
        });

        return await PaginatedList<DecisionTableListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<DecisionTable> ApplySorting(
        IQueryable<DecisionTable> query, string sortBy, string sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "NAME" => isDescending
                ? query.OrderByDescending(dt => dt.Name)
                : query.OrderBy(dt => dt.Name),
            "STATUS" => isDescending
                ? query.OrderByDescending(dt => dt.Status)
                : query.OrderBy(dt => dt.Status),
            "CREATEDAT" => isDescending
                ? query.OrderByDescending(dt => dt.CreatedAt)
                : query.OrderBy(dt => dt.CreatedAt),
            "UPDATEDAT" => isDescending
                ? query.OrderByDescending(dt => dt.UpdatedAt)
                : query.OrderBy(dt => dt.UpdatedAt),
            "PUBLISHEDAT" => isDescending
                ? query.OrderByDescending(dt => dt.PublishedAt)
                : query.OrderBy(dt => dt.PublishedAt),
            _ => query.OrderBy(dt => dt.Name)
        };
    }
}
