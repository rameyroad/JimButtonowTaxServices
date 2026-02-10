using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Queries.ListWorkflowVersions;

public class ListWorkflowVersionsHandler : IRequestHandler<ListWorkflowVersionsQuery, IReadOnlyList<WorkflowVersionListItemDto>>
{
    private readonly DbContext _dbContext;

    public ListWorkflowVersionsHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<WorkflowVersionListItemDto>> Handle(
        ListWorkflowVersionsQuery request,
        CancellationToken cancellationToken)
    {
        var versions = await _dbContext.Set<WorkflowVersion>()
            .Where(v => v.WorkflowDefinitionId == request.WorkflowDefinitionId)
            .OrderByDescending(v => v.VersionNumber)
            .Select(v => new WorkflowVersionListItemDto
            {
                Id = v.Id,
                WorkflowDefinitionId = v.WorkflowDefinitionId,
                VersionNumber = v.VersionNumber,
                PublishedAt = v.PublishedAt,
                PublishedByUserId = v.PublishedByUserId,
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return versions;
    }
}
