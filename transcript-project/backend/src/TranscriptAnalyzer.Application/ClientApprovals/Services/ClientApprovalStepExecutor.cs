using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Workflows.Services;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.ClientApprovals.Services;

public class ClientApprovalStepExecutor : IStepExecutor
{
    private static readonly JsonSerializerOptions CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly DbContext _dbContext;

    public ClientApprovalStepExecutor(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StepResult> ExecuteAsync(
        WorkflowStep workflowStep,
        StepExecution execution,
        CaseWorkflow caseWorkflow,
        CancellationToken cancellationToken = default)
    {
        var title = workflowStep.Name;
        string? description = null;
        int expirationDays = 14;

        if (!string.IsNullOrWhiteSpace(workflowStep.Configuration))
        {
            var config = JsonSerializer.Deserialize<ClientApprovalStepConfig>(
                workflowStep.Configuration,
                CaseInsensitiveOptions);

            if (config is not null)
            {
                title = config.Title ?? title;
                description = config.Description;

                if (config.ExpirationDays > 0)
                {
                    expirationDays = config.ExpirationDays.Value;
                }
            }
        }

        var clientApproval = new ClientApproval(
            caseWorkflow.OrganizationId,
            caseWorkflow.Id,
            execution.Id,
            caseWorkflow.ClientId,
            title,
            description,
            expirationDays);

        _dbContext.Set<ClientApproval>().Add(clientApproval);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StepResult
        {
            Success = true,
            ShouldPause = true,
            OutputData = JsonSerializer.Serialize(new
            {
                clientApprovalId = clientApproval.Id,
                token = clientApproval.Token,
                expiresAt = clientApproval.TokenExpiresAt
            })
        };
    }

#pragma warning disable CA1812 // Instantiated via JSON deserialization
    private sealed record ClientApprovalStepConfig
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public int? ExpirationDays { get; init; }
    }
#pragma warning restore CA1812
}
