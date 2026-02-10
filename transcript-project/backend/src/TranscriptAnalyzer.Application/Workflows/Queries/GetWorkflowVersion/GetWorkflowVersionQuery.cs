using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Queries.GetWorkflowVersion;

public record GetWorkflowVersionQuery(Guid WorkflowDefinitionId, Guid VersionId) : IRequest<WorkflowVersionDetailDto?>;
