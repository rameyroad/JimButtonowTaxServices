using MediatR;
using TranscriptAnalyzer.Application.Workflows.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Commands.UpdateWorkflowStep;

public record UpdateWorkflowStepCommand : IRequest<WorkflowStepDto?>
{
    public Guid WorkflowDefinitionId { get; init; }
    public Guid StepId { get; init; }
    public string? Name { get; init; }
    public WorkflowStepType? StepType { get; init; }
    public int? SortOrder { get; init; }
    public string? Configuration { get; init; }
    public Guid? NextStepOnSuccessId { get; init; }
    public Guid? NextStepOnFailureId { get; init; }
    public bool? IsRequired { get; init; }
}
