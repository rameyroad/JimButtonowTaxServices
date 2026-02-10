using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Commands.PublishWorkflow;

public record PublishWorkflowCommand(Guid Id) : IRequest<WorkflowDefinitionDetailDto?>;
