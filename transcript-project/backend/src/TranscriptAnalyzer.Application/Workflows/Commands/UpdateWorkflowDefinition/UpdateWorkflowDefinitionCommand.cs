using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Commands.UpdateWorkflowDefinition;

public record UpdateWorkflowDefinitionCommand : IRequest<WorkflowDefinitionDetailDto?>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
}
