using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;

namespace TranscriptAnalyzer.Application.Workflows.Commands.CreateWorkflowDefinition;

public record CreateWorkflowDefinitionCommand : IRequest<WorkflowDefinitionDetailDto>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
}
