using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class Invitation : TenantEntity
{
    public string Email { get; private set; }
    public UserRole Role { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public InvitationStatus Status { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public Guid InvitedByUserId { get; private set; }

    public Organization Organization { get; private set; } = null!;

#pragma warning disable CS8618 // Required for EF Core
    private Invitation() { }
#pragma warning restore CS8618

    public Invitation(
        Guid organizationId,
        string email,
        UserRole role,
        Guid invitedByUserId,
        int expirationDays = 7) : base(organizationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        Email = email.Trim().ToLowerInvariant();
        Role = role;
        Token = GenerateToken();
        ExpiresAt = DateTime.UtcNow.AddDays(expirationDays);
        Status = InvitationStatus.Pending;
        InvitedByUserId = invitedByUserId;
    }

    public void Accept()
    {
        Status = InvitationStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Revoke()
    {
        Status = InvitationStatus.Revoked;
        SetUpdatedAt();
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    public bool CanAccept => Status == InvitationStatus.Pending && !IsExpired;

    private static string GenerateToken()
    {
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-", StringComparison.Ordinal)
            .Replace("/", "_", StringComparison.Ordinal)
            .TrimEnd('=');
    }
}
