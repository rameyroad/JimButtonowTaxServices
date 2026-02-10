using FluentValidation;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.CreateDecisionTable;

public class CreateDecisionTableValidator : AbstractValidator<CreateDecisionTableCommand>
{
    public CreateDecisionTableValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => x.Description != null);

        RuleFor(x => x.Columns)
            .NotEmpty().WithMessage("At least one column is required");

        RuleForEach(x => x.Columns).ChildRules(column =>
        {
            column.RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Column name is required")
                .MaximumLength(100).WithMessage("Column name must not exceed 100 characters");

            column.RuleFor(c => c.Key)
                .NotEmpty().WithMessage("Column key is required")
                .MaximumLength(100).WithMessage("Column key must not exceed 100 characters")
                .Matches("^[a-z][a-z0-9_]*$").WithMessage("Column key must be lowercase alphanumeric with underscores, starting with a letter");
        });

        RuleFor(x => x.Columns)
            .Must(columns => columns.Select(c => c.Key).Distinct().Count() == columns.Count)
            .WithMessage("Column keys must be unique")
            .When(x => x.Columns != null && x.Columns.Count > 0);

        RuleFor(x => x.Columns)
            .Must(columns => columns.Any(c => c.IsInput))
            .WithMessage("At least one input column is required")
            .When(x => x.Columns != null && x.Columns.Count > 0);

        RuleFor(x => x.Columns)
            .Must(columns => columns.Any(c => !c.IsInput))
            .WithMessage("At least one output column is required")
            .When(x => x.Columns != null && x.Columns.Count > 0);
    }
}
