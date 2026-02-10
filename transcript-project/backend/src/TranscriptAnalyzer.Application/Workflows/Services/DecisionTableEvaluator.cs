using System.Globalization;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Workflows.Services;

public static class DecisionTableEvaluator
{
    public static Dictionary<string, string>? Evaluate(
        DecisionTable decisionTable,
        Dictionary<string, string> inputs)
    {
        ArgumentNullException.ThrowIfNull(decisionTable);
        ArgumentNullException.ThrowIfNull(inputs);

        // Iterate rules in priority order, return first match
        var orderedRules = decisionTable.Rules
            .Where(r => r.IsEnabled)
            .OrderBy(r => r.Priority);

        foreach (var rule in orderedRules)
        {
            if (AllConditionsMet(rule.Conditions, inputs))
            {
                return rule.Outputs.ToDictionary(o => o.ColumnKey, o => o.Value);
            }
        }

        return null; // No matching rule
    }

    private static bool AllConditionsMet(
        IEnumerable<RuleCondition> conditions,
        Dictionary<string, string> inputs)
    {
        foreach (var condition in conditions)
        {
            if (!inputs.TryGetValue(condition.ColumnKey, out var inputValue))
            {
                // Missing input — only IsEmpty should match
                if (condition.Operator != ConditionOperator.IsEmpty)
                    return false;
                continue;
            }

            if (!EvaluateCondition(condition, inputValue))
                return false;
        }

        return true;
    }

    private static bool EvaluateCondition(RuleCondition condition, string inputValue)
    {
        return condition.Operator switch
        {
            ConditionOperator.Equals => string.Equals(inputValue, condition.Value, StringComparison.OrdinalIgnoreCase),
            ConditionOperator.NotEquals => !string.Equals(inputValue, condition.Value, StringComparison.OrdinalIgnoreCase),
            ConditionOperator.Contains => inputValue.Contains(condition.Value, StringComparison.OrdinalIgnoreCase),
            ConditionOperator.IsEmpty => string.IsNullOrWhiteSpace(inputValue),
            ConditionOperator.IsNotEmpty => !string.IsNullOrWhiteSpace(inputValue),
            ConditionOperator.LessThan => CompareNumeric(inputValue, condition.Value) < 0,
            ConditionOperator.GreaterThan => CompareNumeric(inputValue, condition.Value) > 0,
            ConditionOperator.LessThanOrEqual => CompareNumeric(inputValue, condition.Value) <= 0,
            ConditionOperator.GreaterThanOrEqual => CompareNumeric(inputValue, condition.Value) >= 0,
            ConditionOperator.Between => EvaluateBetween(inputValue, condition.Value, condition.Value2),
            _ => false,
        };
    }

    private static int CompareNumeric(string left, string right)
    {
        if (decimal.TryParse(left, CultureInfo.InvariantCulture, out var leftNum) &&
            decimal.TryParse(right, CultureInfo.InvariantCulture, out var rightNum))
        {
            return leftNum.CompareTo(rightNum);
        }

        // Fall back to string comparison
        return string.Compare(left, right, StringComparison.OrdinalIgnoreCase);
    }

    private static bool EvaluateBetween(string inputValue, string low, string? high)
    {
        if (high is null) return false;

        if (decimal.TryParse(inputValue, CultureInfo.InvariantCulture, out var value) &&
            decimal.TryParse(low, CultureInfo.InvariantCulture, out var lowNum) &&
            decimal.TryParse(high, CultureInfo.InvariantCulture, out var highNum))
        {
            return value >= lowNum && value <= highNum;
        }

        // Fall back to string comparison
        return string.Compare(inputValue, low, StringComparison.OrdinalIgnoreCase) >= 0 &&
               string.Compare(inputValue, high, StringComparison.OrdinalIgnoreCase) <= 0;
    }
}
