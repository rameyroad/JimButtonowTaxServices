using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Commands.StartWorkflow;

public record StartWorkflowCommand : IRequest<CaseWorkflowDetailDto>
{
    public required Guid ClientId { get; init; }
    public required Guid WorkflowDefinitionId { get; init; }
}
