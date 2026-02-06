using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public string? EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    private List<NotificationChannel> _channels = [];
    public IReadOnlyList<NotificationChannel> Channels => _channels.AsReadOnly();
    public DateTime? EmailSentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public Organization? Organization { get; private set; }
    public User? User { get; private set; }

    public bool IsRead => ReadAt.HasValue;

#pragma warning disable CS8618 // Required for EF Core
    private Notification() { }
#pragma warning restore CS8618

    public Notification(
        Guid organizationId,
        Guid userId,
        NotificationType type,
        string title,
        string message,
        IEnumerable<NotificationChannel> channels,
        string? entityType = null,
        Guid? entityId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentNullException.ThrowIfNull(channels);

        OrganizationId = organizationId;
        UserId = userId;
        Type = type;
        Title = title;
        Message = message;
        _channels = channels.ToList();
        EntityType = entityType;
        EntityId = entityId;
    }

    public void MarkAsRead()
    {
        if (!ReadAt.HasValue)
        {
            ReadAt = DateTime.UtcNow;
            SetUpdatedAt();
        }
    }

    public void RecordEmailSent()
    {
        EmailSentAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
