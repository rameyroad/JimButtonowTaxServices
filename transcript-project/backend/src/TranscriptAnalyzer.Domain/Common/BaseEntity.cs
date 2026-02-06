namespace TranscriptAnalyzer.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

public abstract class SoftDeletableEntity : BaseEntity
{
    public DateTime? DeletedAt { get; protected set; }
    public bool IsDeleted => DeletedAt.HasValue;

    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Restore()
    {
        DeletedAt = null;
        SetUpdatedAt();
    }
}

public abstract class TenantEntity : SoftDeletableEntity
{
    public Guid OrganizationId { get; protected set; }

    protected TenantEntity()
    {
    }

    protected TenantEntity(Guid organizationId)
    {
        OrganizationId = organizationId;
    }
}
