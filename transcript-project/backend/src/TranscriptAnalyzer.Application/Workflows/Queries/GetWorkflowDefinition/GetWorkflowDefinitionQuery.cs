using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Queries.GetWorkflowDefinition;

public record GetWorkflowDefinitionQuery(Guid Id) : IRequest<WorkflowDefinitionDetailDto?>;
