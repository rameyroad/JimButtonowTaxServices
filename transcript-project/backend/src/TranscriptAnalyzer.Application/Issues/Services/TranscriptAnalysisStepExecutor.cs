using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Application.Issues.Services;

public class TranscriptAnalysisStepExecutor : IStepExecutor
{
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly DbContext _dbContext;
    private readonly ITranscriptParser _transcriptParser;

    public TranscriptAnalysisStepExecutor(DbContext dbContext, ITranscriptParser transcriptParser)
    {
        _dbContext = dbContext;
        _transcriptParser = transcriptParser;
    }

    public async Task<StepResult> ExecuteAsync(
        WorkflowStep workflowStep,
        StepExecution execution,
        CaseWorkflow caseWorkflow,
        CancellationToken cancellationToken = default)
    {
        // Parse configuration for optional settings
        var config = !string.IsNullOrWhiteSpace(workflowStep.Configuration)
            ? JsonSerializer.Deserialize<TranscriptAnalysisConfig>(
                workflowStep.Configuration, CaseInsensitiveOptions)
            : null;

        // Load transcripts for this client
        var transcripts = await _dbContext.Set<Transcript>()
            .Where(t => t.ClientId == caseWorkflow.ClientId)
            .ToListAsync(cancellationToken);

        if (transcripts.Count == 0)
        {
            return new StepResult
            {
                Success = true,
                OutputData = JsonSerializer.Serialize(new
                {
                    issuesDetected = 0,
                    message = "No transcripts found for client."
                })
            };
        }

        // Parse each transcript and collect entries
        var allEntries = new List<TranscriptEntry>();
        foreach (var transcript in transcripts)
        {
            // Filter by tax year if configured
            if (config?.TaxYear.HasValue == true && transcript.TaxYear != config.TaxYear.Value)
                continue;

            // In a real implementation, we'd read the transcript content from blob storage
            // For now, we use the input data if provided or skip
            if (!string.IsNullOrWhiteSpace(execution.InputData))
            {
                var inputEntries = JsonSerializer.Deserialize<List<TranscriptEntry>>(
                    execution.InputData, CaseInsensitiveOptions);
                if (inputEntries is not null)
                    allEntries.AddRange(inputEntries);
            }
        }

        // Detect issues from transcript entries
        var issues = IssueDetectionService.DetectIssues(
            caseWorkflow.OrganizationId,
            caseWorkflow.ClientId,
            allEntries,
            caseWorkflow.Id);

        // Store detected issues
        foreach (var issue in issues)
        {
            _dbContext.Set<Issue>().Add(issue);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StepResult
        {
            Success = true,
            OutputData = JsonSerializer.Serialize(new
            {
                issuesDetected = issues.Count,
                byType = issues.GroupBy(i => i.IssueType.ToString())
                    .Select(g => new { type = g.Key, count = g.Count() })
            })
        };
    }

#pragma warning disable CA1812 // Instantiated via JSON deserialization
    private sealed record TranscriptAnalysisConfig
    {
        public int? TaxYear { get; init; }
    }
#pragma warning restore CA1812
}
