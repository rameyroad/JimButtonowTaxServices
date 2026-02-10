using FluentValidation;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.UpdateDecisionRule;

public class UpdateDecisionRuleValidator : AbstractValidator<UpdateDecisionRuleCommand>
{
    public UpdateDecisionRuleValidator()
    {
        RuleFor(x => x.DecisionTableId)
            .NotEmpty().WithMessage("Decision table ID is required");

        RuleFor(x => x.RuleId)
            .NotEmpty().WithMessage("Rule ID is required");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority must be non-negative")
            .When(x => x.Priority.HasValue);

        When(x => x.Conditions != null, () =>
        {
            RuleFor(x => x.Conditions!)
                .NotEmpty().WithMessage("Conditions list cannot be empty when provided");

            RuleForEach(x => x.Conditions).ChildRules(condition =>
            {
                condition.RuleFor(c => c.ColumnKey)
                    .NotEmpty().WithMessage("Column key is required");

                condition.RuleFor(c => c.Value)
                    .NotNull().WithMessage("Value is required");
            });
        });

        When(x => x.Outputs != null, () =>
        {
            RuleFor(x => x.Outputs!)
                .NotEmpty().WithMessage("Outputs list cannot be empty when provided");

            RuleForEach(x => x.Outputs).ChildRules(output =>
            {
                output.RuleFor(o => o.ColumnKey)
                    .NotEmpty().WithMessage("Column key is required");

                output.RuleFor(o => o.Value)
                    .NotNull().WithMessage("Value is required");
            });
        });
    }
}
