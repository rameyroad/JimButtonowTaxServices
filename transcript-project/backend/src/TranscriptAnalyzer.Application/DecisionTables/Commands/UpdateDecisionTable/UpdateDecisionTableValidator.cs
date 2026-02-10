using FluentValidation;
using TranscriptAnalyzer.Application.DecisionTables.Commands.CreateDecisionTable;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionTable;

public class UpdateDecisionTableValidator : AbstractValidator<UpdateDecisionTableCommand>
{
    public UpdateDecisionTableValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
            .When(x => x.Name != null);

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => x.Description != null);

        When(x => x.Columns != null && x.Columns.Count > 0, () =>
        {
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

            RuleFor(x => x.Columns!)
                .Must(columns => columns.Select(c => c.Key).Distinct().Count() == columns.Count)
                .WithMessage("Column keys must be unique");

            RuleFor(x => x.Columns!)
                .Must(columns => columns.Any(c => c.IsInput))
                .WithMessage("At least one input column is required");

            RuleFor(x => x.Columns!)
                .Must(columns => columns.Any(c => !c.IsInput))
                .WithMessage("At least one output column is required");
        });
    }
}
