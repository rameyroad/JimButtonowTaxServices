using FluentValidation;

namespace TranscriptAnalyzer.Application.Formulas.Commands.CreateFormula;

public class CreateFormulaValidator : AbstractValidator<CreateFormulaCommand>
{
    public CreateFormulaValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => x.Description != null);

        RuleFor(x => x.Expression)
            .NotEmpty().WithMessage("Expression is required")
            .MaximumLength(4000).WithMessage("Expression must not exceed 4000 characters");

        RuleFor(x => x.InputVariables)
            .NotEmpty().WithMessage("Input variables are required")
            .MaximumLength(4000).WithMessage("Input variables must not exceed 4000 characters");

        RuleFor(x => x.OutputType)
            .IsInEnum().WithMessage("Invalid output type");
    }
}
