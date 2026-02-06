using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? UserId { get; private set; }
    public AuditAction Action { get; private set; }
    public string EntityType { get; private set; }
    public Guid EntityId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? BeforeState { get; private set; }
    public string? AfterState { get; private set; }
    public string? Metadata { get; private set; }
    public DateTime Timestamp { get; private set; }

    public Organization? Organization { get; private set; }
    public User? User { get; private set; }

#pragma warning disable CS8618 // Required for EF Core
    private AuditLog() { }
#pragma warning restore CS8618

    public AuditLog(
        Guid organizationId,
        AuditAction action,
        string entityType,
        Guid entityId,
        Guid? userId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? beforeState = null,
        string? afterState = null,
        string? metadata = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityType);

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        UserId = userId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        IpAddress = ipAddress;
        UserAgent = userAgent?.Length > 500 ? userAgent[..500] : userAgent;
        BeforeState = beforeState;
        AfterState = afterState;
        Metadata = metadata;
        Timestamp = DateTime.UtcNow;
    }
}
