using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class ClientApproval : TenantEntity
{
    public Guid CaseWorkflowId { get; private set; }
    public Guid StepExecutionId { get; private set; }
    public Guid ClientId { get; private set; }
    public string Token { get; private set; }
    public DateTime TokenExpiresAt { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public ClientApprovalStatus Status { get; private set; }
    public DateTime? RespondedAt { get; private set; }
    public string? ResponseNotes { get; private set; }
    public DateTime? ReminderSentAt { get; private set; }

    public CaseWorkflow CaseWorkflow { get; private set; } = null!;
    public StepExecution StepExecution { get; private set; } = null!;
    public Client Client { get; private set; } = null!;

#pragma warning disable CS8618 // Required for EF Core
    private ClientApproval() { }
#pragma warning restore CS8618

    public ClientApproval(
        Guid organizationId,
        Guid caseWorkflowId,
        Guid stepExecutionId,
        Guid clientId,
        string title,
        string? description = null,
        int expirationDays = 14) : base(organizationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        CaseWorkflowId = caseWorkflowId;
        StepExecutionId = stepExecutionId;
        ClientId = clientId;
        Title = title.Trim();
        Description = description?.Trim();
        Token = GenerateToken();
        TokenExpiresAt = DateTime.UtcNow.AddDays(expirationDays);
        Status = ClientApprovalStatus.Pending;
    }

    public void Approve(string? notes = null)
    {
        Status = ClientApprovalStatus.Approved;
        RespondedAt = DateTime.UtcNow;
        ResponseNotes = notes?.Trim();
        SetUpdatedAt();
    }

    public void Decline(string? notes = null)
    {
        Status = ClientApprovalStatus.Declined;
        RespondedAt = DateTime.UtcNow;
        ResponseNotes = notes?.Trim();
        SetUpdatedAt();
    }

    public void MarkExpired()
    {
        Status = ClientApprovalStatus.Expired;
        SetUpdatedAt();
    }

    public void RecordReminderSent()
    {
        ReminderSentAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public bool IsExpired => DateTime.UtcNow > TokenExpiresAt;

    public bool CanRespond => Status == ClientApprovalStatus.Pending && !IsExpired;

    private static string GenerateToken()
    {
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .TrimEnd('=');
    }
}
