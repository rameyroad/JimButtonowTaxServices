namespace TranscriptAnalyzer.Domain.Enums;

public enum NotificationType
{
    AuthorizationSigned = 0,
    AuthorizationExpiring = 1,
    AuthorizationExpired = 2,
    TranscriptUploaded = 3,
    TeamInvitation = 4,
    TeamMemberJoined = 5
}
