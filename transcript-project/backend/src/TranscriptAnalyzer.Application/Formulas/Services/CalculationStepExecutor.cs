using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Formulas.Services;

public class CalculationStepExecutor : IStepExecutor
{
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly DbContext _dbContext;

    public CalculationStepExecutor(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StepResult> ExecuteAsync(
        WorkflowStep workflowStep,
        StepExecution execution,
        CaseWorkflow caseWorkflow,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workflowStep.Configuration))
        {
            return new StepResult
            {
                Success = false,
                ErrorMessage = "Calculation step has no configuration."
            };
        }

        var config = JsonSerializer.Deserialize<CalculationStepConfig>(
            workflowStep.Configuration,
            CaseInsensitiveOptions);

        if (config?.FormulaId is null)
        {
            return new StepResult
            {
                Success = false,
                ErrorMessage = "Calculation step configuration is missing FormulaId."
            };
        }

        // Load the formula
        var formula = await _dbContext.Set<CalculationFormula>()
            .FirstOrDefaultAsync(f => f.Id == config.FormulaId && f.Status == PublishStatus.Published,
                cancellationToken);

        if (formula is null)
        {
            return new StepResult
            {
                Success = false,
                ErrorMessage = $"Published calculation formula {config.FormulaId} not found."
            };
        }

        // Parse input data from the step execution
        var inputs = new Dictionary<string, decimal>();
        if (!string.IsNullOrWhiteSpace(execution.InputData))
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                execution.InputData, CaseInsensitiveOptions);
            if (parsed is not null)
            {
                foreach (var kvp in parsed)
                {
                    if (kvp.Value.TryGetDecimal(out var decimalValue))
                    {
                        inputs[kvp.Key] = decimalValue;
                    }
                    else if (decimal.TryParse(
                        kvp.Value.GetString(),
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out var parsedDecimal))
                    {
                        inputs[kvp.Key] = parsedDecimal;
                    }
                }
            }
        }

        // Evaluate the formula
        var result = FormulaEvaluator.Evaluate(formula.Expression, inputs);

        return new StepResult
        {
            Success = true,
            OutputData = JsonSerializer.Serialize(new
            {
                formulaId = formula.Id,
                formulaName = formula.Name,
                result,
                expression = formula.Expression
            })
        };
    }

#pragma warning disable CA1812 // Instantiated via JSON deserialization
    private sealed record CalculationStepConfig
    {
        public Guid? FormulaId { get; init; }
    }
#pragma warning restore CA1812
}
