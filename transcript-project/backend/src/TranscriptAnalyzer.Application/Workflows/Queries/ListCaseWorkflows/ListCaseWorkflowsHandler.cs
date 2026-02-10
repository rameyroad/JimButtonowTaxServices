using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Queries.ListCaseWorkflows;

public class ListCaseWorkflowsHandler : IRequestHandler<ListCaseWorkflowsQuery, PaginatedList<CaseWorkflowListItemDto>>
{
    private readonly DbContext _dbContext;

    public ListCaseWorkflowsHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedList<CaseWorkflowListItemDto>> Handle(
        ListCaseWorkflowsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<CaseWorkflow>()
            .Include(cw => cw.WorkflowDefinition)
            .Include(cw => cw.StepExecutions)
            .Where(cw => cw.ClientId == request.ClientId);

        // Filter by status
        if (request.Status.HasValue)
        {
            query = query.Where(cw => cw.Status == request.Status.Value);
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        var dtoQuery = query.Select(cw => new CaseWorkflowListItemDto
        {
            Id = cw.Id,
            ClientId = cw.ClientId,
            WorkflowName = cw.WorkflowDefinition.Name,
            WorkflowVersion = cw.WorkflowVersion,
            Status = cw.Status,
            StartedAt = cw.StartedAt,
            CompletedAt = cw.CompletedAt,
            TotalSteps = cw.WorkflowDefinition.Steps.Count,
            CompletedSteps = cw.StepExecutions.Count(se => se.Status == StepExecutionStatus.Completed),
            CreatedAt = cw.CreatedAt
        });

        return await PaginatedList<CaseWorkflowListItemDto>.CreateAsync(
            dtoQuery,
            request.Page,
            request.PageSize,
            cancellationToken);
    }

    private static IQueryable<CaseWorkflow> ApplySorting(
        IQueryable<CaseWorkflow> query, string sortBy, string sortOrder)
    {
        var isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToUpperInvariant() switch
        {
            "STATUS" => isDescending
                ? query.OrderByDescending(cw => cw.Status)
                : query.OrderBy(cw => cw.Status),
            "STARTEDAT" => isDescending
                ? query.OrderByDescending(cw => cw.StartedAt)
                : query.OrderBy(cw => cw.StartedAt),
            "COMPLETEDAT" => isDescending
                ? query.OrderByDescending(cw => cw.CompletedAt)
                : query.OrderBy(cw => cw.CompletedAt),
            _ => isDescending
                ? query.OrderByDescending(cw => cw.CreatedAt)
                : query.OrderBy(cw => cw.CreatedAt)
        };
    }
}
