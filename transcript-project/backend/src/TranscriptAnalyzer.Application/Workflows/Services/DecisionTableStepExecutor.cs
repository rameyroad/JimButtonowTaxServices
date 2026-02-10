using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Workflows.Services;

public class DecisionTableStepExecutor : IStepExecutor
{
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly DbContext _dbContext;

    public DecisionTableStepExecutor(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StepResult> ExecuteAsync(
        WorkflowStep workflowStep,
        StepExecution execution,
        CaseWorkflow caseWorkflow,
        CancellationToken cancellationToken = default)
    {
        // Parse configuration to get the decision table ID
        if (string.IsNullOrWhiteSpace(workflowStep.Configuration))
        {
            return new StepResult
            {
                Success = false,
                ErrorMessage = "Decision table step has no configuration."
            };
        }

        var config = JsonSerializer.Deserialize<DecisionTableStepConfig>(
            workflowStep.Configuration,
            CaseInsensitiveOptions);

        if (config?.DecisionTableId is null)
        {
            return new StepResult
            {
                Success = false,
                ErrorMessage = "Decision table step configuration is missing DecisionTableId."
            };
        }

        // Load the decision table
        var decisionTable = await _dbContext.Set<DecisionTable>()
            .Include(dt => dt.Columns)
            .Include(dt => dt.Rules.Where(r => r.IsEnabled))
                .ThenInclude(r => r.Conditions)
            .Include(dt => dt.Rules.Where(r => r.IsEnabled))
                .ThenInclude(r => r.Outputs)
            .FirstOrDefaultAsync(dt => dt.Id == config.DecisionTableId, cancellationToken);

        if (decisionTable is null)
        {
            return new StepResult
            {
                Success = false,
                ErrorMessage = $"Decision table {config.DecisionTableId} not found."
            };
        }

        // Parse input data from the step execution
        var inputs = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(execution.InputData))
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(execution.InputData);
            if (parsed is not null)
                inputs = parsed;
        }

        // Evaluate the decision table
        var outputs = DecisionTableEvaluator.Evaluate(decisionTable, inputs);

        if (outputs is null)
        {
            return new StepResult
            {
                Success = true,
                OutputData = JsonSerializer.Serialize(new { matched = false, message = "No matching rule found." })
            };
        }

        return new StepResult
        {
            Success = true,
            OutputData = JsonSerializer.Serialize(new { matched = true, results = outputs })
        };
    }

#pragma warning disable CA1812 // Instantiated via JSON deserialization
    private sealed record DecisionTableStepConfig
    {
        public Guid? DecisionTableId { get; init; }
    }
#pragma warning restore CA1812
}
