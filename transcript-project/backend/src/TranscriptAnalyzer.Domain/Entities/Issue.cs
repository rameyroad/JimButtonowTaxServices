using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class Issue : TenantEntity
{
    public Guid ClientId { get; private set; }
    public Guid? CaseWorkflowId { get; private set; }
    public IssueType IssueType { get; private set; }
    public IssueSeverity Severity { get; private set; }
    public int TaxYear { get; private set; }
    public decimal? Amount { get; private set; }
    public string Description { get; private set; }
    public string? TransactionCode { get; private set; }
    public DateTime DetectedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public Guid? ResolvedByUserId { get; private set; }

    public Client Client { get; private set; } = null!;
    public CaseWorkflow? CaseWorkflow { get; private set; }

#pragma warning disable CS8618 // Required for EF Core
    private Issue() { }
#pragma warning restore CS8618

    public Issue(
        Guid organizationId,
        Guid clientId,
        IssueType issueType,
        IssueSeverity severity,
        int taxYear,
        string description,
        decimal? amount = null,
        string? transactionCode = null,
        Guid? caseWorkflowId = null) : base(organizationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        ClientId = clientId;
        CaseWorkflowId = caseWorkflowId;
        IssueType = issueType;
        Severity = severity;
        TaxYear = taxYear;
        Amount = amount;
        Description = description.Trim();
        TransactionCode = transactionCode?.Trim();
        DetectedAt = DateTime.UtcNow;
    }

    public void UpdateSeverity(IssueSeverity severity)
    {
        Severity = severity;
        SetUpdatedAt();
    }

    public void UpdateDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Description = description.Trim();
        SetUpdatedAt();
    }

    public void UpdateAmount(decimal? amount)
    {
        Amount = amount;
        SetUpdatedAt();
    }

    public void Resolve(Guid resolvedByUserId)
    {
        ResolvedAt = DateTime.UtcNow;
        ResolvedByUserId = resolvedByUserId;
        SetUpdatedAt();
    }

    public void Unresolve()
    {
        ResolvedAt = null;
        ResolvedByUserId = null;
        SetUpdatedAt();
    }
}
