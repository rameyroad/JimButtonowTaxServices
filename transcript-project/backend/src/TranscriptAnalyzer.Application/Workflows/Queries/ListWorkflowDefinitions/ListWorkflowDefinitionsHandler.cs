using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Queries.ListWorkflowDefinitions;

public class ListWorkflowDefinitionsHandler : IRequestHandler<ListWorkflowDefinitionsQuery, PaginatedList<WorkflowDefinitionListItemDto>>
{
    private readonly DbContext _dbContext;

    public ListWorkflowDefinitionsHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<WorkflowDefinitionListItemDto>> Handle(
        ListWorkflowDefinitionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<WorkflowDefinition>()
            .Include(wd => wd.Steps)
            .AsQueryable();

        // Subscriber users only see published workflows
        if (request.PublishedOnly)
        {
            query = query.Where(wd => wd.Status == PublishStatus.Published);
        }

        // Filter by status
        if (request.Status.HasValue)
        {
            query = query.Where(wd => wd.Status == request.Status.Value);
        }

        // Filter by category
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(wd => wd.Category == request.Category.Trim());
        }

        // Search by name or description
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchPattern = $"%{request.Search.Trim()}%";
#pragma warning disable CA1307, CA1304
            query = query.Where(wd =>
                EF.Functions.Like(wd.Name, searchPattern) ||
                EF.Functions.Like(wd.Description ?? string.Empty, searchPattern));
#pragma warning restore CA1307, CA1304
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        var dtoQuery = query.Select(wd => new WorkflowDefinitionListItemDto
        {
            Id = wd.Id,
            Name = wd.Name,
            Description = wd.Description,
            Category = wd.Category,
            Status = wd.Status,
            StepCount = wd.Steps.Count,
            CurrentVersion = wd.CurrentVersion,
            PublishedAt = wd.PublishedAt,
            CreatedAt = wd.CreatedAt,
            UpdatedAt = wd.UpdatedAt
        });

        return await PaginatedList<WorkflowDefinitionListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<WorkflowDefinition> ApplySorting(
        IQueryable<WorkflowDefinition> query, string sortBy, string sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "NAME" => isDescending
                ? query.OrderByDescending(wd => wd.Name)
                : query.OrderBy(wd => wd.Name),
            "STATUS" => isDescending
                ? query.OrderByDescending(wd => wd.Status)
                : query.OrderBy(wd => wd.Status),
            "CATEGORY" => isDescending
                ? query.OrderByDescending(wd => wd.Category)
                : query.OrderBy(wd => wd.Category),
            "CREATEDAT" => isDescending
                ? query.OrderByDescending(wd => wd.CreatedAt)
                : query.OrderBy(wd => wd.CreatedAt),
            "UPDATEDAT" => isDescending
                ? query.OrderByDescending(wd => wd.UpdatedAt)
                : query.OrderBy(wd => wd.UpdatedAt),
            "PUBLISHEDAT" => isDescending
                ? query.OrderByDescending(wd => wd.PublishedAt)
                : query.OrderBy(wd => wd.PublishedAt),
            _ => query.OrderBy(wd => wd.Name)
        };
    }
}
