using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.HumanTasks.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.HumanTasks.Queries.ListHumanTasks;

public class ListHumanTasksHandler : IRequestHandler<ListHumanTasksQuery, PaginatedList<HumanTaskListItemDto>>
{
    private readonly DbContext _dbContext;

    public ListHumanTasksHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<HumanTaskListItemDto>> Handle(
        ListHumanTasksQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<HumanTask>().AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        if (request.AssignedToUserId.HasValue)
        {
            query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId.Value);
        }

        query = ApplySorting(query, request.SortBy, request.SortOrder);

        var dtoQuery = query.Select(t => new HumanTaskListItemDto
        {
            Id = t.Id,
            CaseWorkflowId = t.CaseWorkflowId,
            StepExecutionId = t.StepExecutionId,
            AssignedToUserId = t.AssignedToUserId,
            Title = t.Title,
            DueDate = t.DueDate,
            Status = t.Status,
            CompletedAt = t.CompletedAt,
            Decision = t.Decision,
            CreatedAt = t.CreatedAt
        });

        return await PaginatedList<HumanTaskListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<HumanTask> ApplySorting(
        IQueryable<HumanTask> query, string sortBy, string sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "STATUS" => isDescending
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            "DUEDATE" => isDescending
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),
            "TITLE" => isDescending
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            _ => isDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };
    }
}
