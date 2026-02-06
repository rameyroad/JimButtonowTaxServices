using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class Transcript : TenantEntity
{
    public Guid ClientId { get; private set; }
    public Guid AuthorizationId { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public TranscriptType TranscriptType { get; private set; }
    public int TaxYear { get; private set; }
    public string BlobPath { get; private set; }
    public string FileName { get; private set; }
    public long FileSize { get; private set; }
    public string ContentType { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public DateTime? LastAccessedAt { get; private set; }
    public Guid? LastAccessedByUserId { get; private set; }

    public Organization? Organization { get; private set; }
    public Client? Client { get; private set; }
    public Authorization? Authorization { get; private set; }
    public User? UploadedBy { get; private set; }
    public User? LastAccessedBy { get; private set; }

#pragma warning disable CS8618 // Required for EF Core
    private Transcript() { }
#pragma warning restore CS8618

    public Transcript(
        Guid organizationId,
        Guid clientId,
        Guid authorizationId,
        Guid uploadedByUserId,
        TranscriptType transcriptType,
        int taxYear,
        string blobPath,
        string fileName,
        long fileSize,
        string contentType)
        : base(organizationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        if (fileSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fileSize), "File size must be positive.");
        }

        ClientId = clientId;
        AuthorizationId = authorizationId;
        UploadedByUserId = uploadedByUserId;
        TranscriptType = transcriptType;
        TaxYear = taxYear;
        BlobPath = blobPath;
        FileName = fileName;
        FileSize = fileSize;
        ContentType = contentType;
        UploadedAt = DateTime.UtcNow;
    }

    public void RecordAccess(Guid userId)
    {
        LastAccessedAt = DateTime.UtcNow;
        LastAccessedByUserId = userId;
        SetUpdatedAt();
    }
}
