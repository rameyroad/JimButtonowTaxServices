using FluentValidation;

namespace TranscriptAnalyzer.Application.Workflows.Commands.StartWorkflow;

public class StartWorkflowValidator : AbstractValidator<StartWorkflowCommand>
{
    public StartWorkflowValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("Client ID is required");

        RuleFor(x => x.WorkflowDefinitionId)
            .NotEmpty().WithMessage("Workflow definition ID is required");
    }
}
