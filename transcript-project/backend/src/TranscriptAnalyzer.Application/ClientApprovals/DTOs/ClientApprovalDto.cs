using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.ClientApprovals.DTOs;

public record ClientApprovalDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required ClientApprovalStatus Status { get; init; }
    public required DateTime TokenExpiresAt { get; init; }
    public DateTime? RespondedAt { get; init; }
    public string? ResponseNotes { get; init; }
    public required DateTime CreatedAt { get; init; }
}
