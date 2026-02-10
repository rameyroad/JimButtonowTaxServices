using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Commands.AddWorkflowStep;

public record AddWorkflowStepCommand : IRequest<WorkflowStepDto?>
{
    public Guid WorkflowDefinitionId { get; init; }
    public required string Name { get; init; }
    public required WorkflowStepType StepType { get; init; }
    public int SortOrder { get; init; }
    public string? Configuration { get; init; }
    public Guid? NextStepOnSuccessId { get; init; }
    public Guid? NextStepOnFailureId { get; init; }
    public bool IsRequired { get; init; } = true;
}
