using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Queries.ListWorkflowVersions;

public record ListWorkflowVersionsQuery(Guid WorkflowDefinitionId) : IRequest<IReadOnlyList<WorkflowVersionListItemDto>>;
