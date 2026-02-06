using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class Authorization : TenantEntity
{
    public Guid ClientId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public AuthorizationStatus Status { get; private set; }
    private List<int> _taxYears = [];
    public IReadOnlyList<int> TaxYears => _taxYears.AsReadOnly();
    public string? SignatureRequestToken { get; private set; }
    public DateTime? SignatureRequestExpiresAt { get; private set; }
    public string? SignatureData { get; private set; }
    public DateTime? SignedAt { get; private set; }
    public string? SignedByIp { get; private set; }
    public string? SignedByUserAgent { get; private set; }
    public string? ExternalSignatureId { get; private set; }
    public string? FormBlobPath { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public DateTime? CafSubmissionDate { get; private set; }
    public DateTime? CafConfirmationDate { get; private set; }
    public string? CafReferenceNumber { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }

    public Organization? Organization { get; private set; }
    public Client? Client { get; private set; }
    public User? CreatedBy { get; private set; }

    private readonly List<Transcript> _transcripts = [];
    public IReadOnlyCollection<Transcript> Transcripts => _transcripts.AsReadOnly();

    public bool IsActive => Status == AuthorizationStatus.Active && ExpirationDate > DateTime.UtcNow;
    public int? DaysUntilExpiration => ExpirationDate.HasValue
        ? (int)(ExpirationDate.Value - DateTime.UtcNow).TotalDays
        : null;

#pragma warning disable CS8618 // Required for EF Core
    private Authorization() { }
#pragma warning restore CS8618

    public Authorization(
        Guid organizationId,
        Guid clientId,
        Guid createdByUserId,
        IEnumerable<int> taxYears)
        : base(organizationId)
    {
        ArgumentNullException.ThrowIfNull(taxYears);

        var yearsList = taxYears.ToList();
        if (yearsList.Count == 0 || yearsList.Count > 4)
        {
            throw new ArgumentException("Tax years must contain 1 to 4 years.", nameof(taxYears));
        }

        ClientId = clientId;
        CreatedByUserId = createdByUserId;
        _taxYears = yearsList;
        Status = AuthorizationStatus.Draft;
    }

    public void SendForSignature(string signatureRequestToken, int expirationDays)
    {
        if (Status != AuthorizationStatus.Draft)
        {
            throw new InvalidOperationException($"Cannot send for signature from status {Status}.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(signatureRequestToken);

        SignatureRequestToken = signatureRequestToken;
        SignatureRequestExpiresAt = DateTime.UtcNow.AddDays(expirationDays);
        Status = AuthorizationStatus.PendingSignature;
        SetUpdatedAt();
    }

    public void ResendForSignature(string signatureRequestToken, int expirationDays)
    {
        if (Status != AuthorizationStatus.PendingSignature)
        {
            throw new InvalidOperationException($"Cannot resend from status {Status}.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(signatureRequestToken);

        SignatureRequestToken = signatureRequestToken;
        SignatureRequestExpiresAt = DateTime.UtcNow.AddDays(expirationDays);
        SetUpdatedAt();
    }

    public void RecordSignature(
        string signatureData,
        string signedByIp,
        string? signedByUserAgent = null,
        string? externalSignatureId = null)
    {
        if (Status != AuthorizationStatus.PendingSignature)
        {
            throw new InvalidOperationException($"Cannot record signature from status {Status}.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(signatureData);
        ArgumentException.ThrowIfNullOrWhiteSpace(signedByIp);

        SignatureData = signatureData;
        SignedAt = DateTime.UtcNow;
        SignedByIp = signedByIp;
        SignedByUserAgent = signedByUserAgent;
        ExternalSignatureId = externalSignatureId;
        ExpirationDate = DateTime.UtcNow.AddYears(3);
        Status = AuthorizationStatus.Signed;
        SignatureRequestToken = null;
        SignatureRequestExpiresAt = null;
        SetUpdatedAt();
    }

    public void SetFormBlobPath(string formBlobPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(formBlobPath);
        FormBlobPath = formBlobPath;
        SetUpdatedAt();
    }

    public void SubmitToCaf(DateTime submissionDate)
    {
        if (Status != AuthorizationStatus.Signed)
        {
            throw new InvalidOperationException($"Cannot submit to CAF from status {Status}.");
        }

        CafSubmissionDate = submissionDate;
        Status = AuthorizationStatus.SubmittedToCaf;
        SetUpdatedAt();
    }

    public void ConfirmCaf(DateTime confirmationDate, string referenceNumber)
    {
        if (Status != AuthorizationStatus.SubmittedToCaf)
        {
            throw new InvalidOperationException($"Cannot confirm CAF from status {Status}.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);

        CafConfirmationDate = confirmationDate;
        CafReferenceNumber = referenceNumber;
        Status = AuthorizationStatus.Active;
        SetUpdatedAt();
    }

    public void MarkExpired()
    {
        if (Status != AuthorizationStatus.Active)
        {
            throw new InvalidOperationException($"Cannot mark expired from status {Status}.");
        }

        Status = AuthorizationStatus.Expired;
        SetUpdatedAt();
    }

    public void Revoke(string reason)
    {
        if (Status == AuthorizationStatus.Revoked || Status == AuthorizationStatus.Expired)
        {
            throw new InvalidOperationException($"Cannot revoke from status {Status}.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
        Status = AuthorizationStatus.Revoked;
        SetUpdatedAt();
    }

    public void RevertToDraft()
    {
        if (Status != AuthorizationStatus.PendingSignature)
        {
            throw new InvalidOperationException($"Cannot revert to draft from status {Status}.");
        }

        SignatureRequestToken = null;
        SignatureRequestExpiresAt = null;
        Status = AuthorizationStatus.Draft;
        SetUpdatedAt();
    }
}
