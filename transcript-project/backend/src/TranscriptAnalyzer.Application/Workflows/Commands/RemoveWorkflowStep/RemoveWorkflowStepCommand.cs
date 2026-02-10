using MediatR;

namespace TranscriptAnalyzer.Application.Workflows.Commands.RemoveWorkflowStep;

public record RemoveWorkflowStepCommand(Guid WorkflowDefinitionId, Guid StepId) : IRequest<bool>;
