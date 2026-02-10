using FluentValidation;

namespace TranscriptAnalyzer.Application.DecisionTables.Commands.AddDecisionRule;

public class AddDecisionRuleValidator : AbstractValidator<AddDecisionRuleCommand>
{
    public AddDecisionRuleValidator()
    {
        RuleFor(x => x.DecisionTableId)
            .NotEmpty().WithMessage("Decision table ID is required");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority must be non-negative");

        RuleFor(x => x.Conditions)
            .NotEmpty().WithMessage("At least one condition is required");

        RuleForEach(x => x.Conditions).ChildRules(condition =>
        {
            condition.RuleFor(c => c.ColumnKey)
                .NotEmpty().WithMessage("Column key is required");

            condition.RuleFor(c => c.Value)
                .NotNull().WithMessage("Value is required");

            condition.RuleFor(c => c.Value2)
                .NotNull().WithMessage("Value2 is required for Between operator")
                .When(c => c.Operator == Domain.Enums.ConditionOperator.Between);
        });

        RuleFor(x => x.Outputs)
            .NotEmpty().WithMessage("At least one output is required");

        RuleForEach(x => x.Outputs).ChildRules(output =>
        {
            output.RuleFor(o => o.ColumnKey)
                .NotEmpty().WithMessage("Column key is required");

            output.RuleFor(o => o.Value)
                .NotNull().WithMessage("Value is required");
        });
    }
}
