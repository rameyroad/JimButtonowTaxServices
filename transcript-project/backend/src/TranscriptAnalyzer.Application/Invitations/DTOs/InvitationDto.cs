using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Invitations.DTOs;

public record InvitationDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
    public required InvitationStatus Status { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public DateTime? AcceptedAt { get; init; }
    public required Guid InvitedByUserId { get; init; }
    public required DateTime CreatedAt { get; init; }
}
