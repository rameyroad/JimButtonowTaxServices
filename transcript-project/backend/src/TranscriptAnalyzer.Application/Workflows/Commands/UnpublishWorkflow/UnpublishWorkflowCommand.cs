using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Commands.UnpublishWorkflow;

public record UnpublishWorkflowCommand(Guid Id) : IRequest<WorkflowDefinitionDetailDto?>;
