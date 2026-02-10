using TranscriptAnalyzer.Domain.Common;

namespace TranscriptAnalyzer.Domain.Entities;

public class WorkflowVersion : SoftDeletableEntity
{
    public Guid WorkflowDefinitionId { get; private set; }
    public int VersionNumber { get; private set; }
    public DateTime PublishedAt { get; private set; }
    public Guid PublishedByUserId { get; private set; }
    public string SnapshotData { get; private set; }
    public bool IsActive { get; private set; }

    public WorkflowDefinition WorkflowDefinition { get; private set; } = null!;

#pragma warning disable CS8618 // Required for EF Core
    private WorkflowVersion() { }
#pragma warning restore CS8618

    public WorkflowVersion(
        Guid workflowDefinitionId,
        int versionNumber,
        Guid publishedByUserId,
        string snapshotData)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshotData);

        WorkflowDefinitionId = workflowDefinitionId;
        VersionNumber = versionNumber;
        PublishedByUserId = publishedByUserId;
        PublishedAt = DateTime.UtcNow;
        SnapshotData = snapshotData;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }
}
