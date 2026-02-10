using FluentValidation;

namespace TranscriptAnalyzer.Application.Workflows.Commands.AddWorkflowStep;

public class AddWorkflowStepValidator : AbstractValidator<AddWorkflowStepCommand>
{
    public AddWorkflowStepValidator()
    {
        RuleFor(x => x.WorkflowDefinitionId)
            .NotEmpty().WithMessage("Workflow definition ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.StepType)
            .IsInEnum().WithMessage("Invalid step type");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort order must be non-negative");

        RuleFor(x => x.Configuration)
            .MaximumLength(4000).WithMessage("Configuration must not exceed 4000 characters")
            .When(x => x.Configuration != null);
    }
}
