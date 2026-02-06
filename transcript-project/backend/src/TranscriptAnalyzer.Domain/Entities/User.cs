using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Domain.Entities;

public class User : TenantEntity
{
    public string Auth0UserId { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime? InvitedAt { get; private set; }
    public Guid? InvitedByUserId { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public Organization? Organization { get; private set; }
    public User? InvitedBy { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

#pragma warning disable CS8618 // Required for EF Core
    private User() { }
#pragma warning restore CS8618

    public User(
        Guid organizationId,
        string auth0UserId,
        string email,
        string firstName,
        string lastName,
        UserRole role)
        : base(organizationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(auth0UserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        Auth0UserId = auth0UserId;
        Email = email.Trim().ToLowerInvariant();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Role = role;
        Status = UserStatus.Active;
    }

    public static User CreateInvitedUser(
        Guid organizationId,
        string email,
        string firstName,
        string lastName,
        UserRole role,
        Guid invitedByUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        var user = new User
        {
            OrganizationId = organizationId,
            Auth0UserId = string.Empty,
            Email = email.Trim().ToLowerInvariant(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Role = role,
            Status = UserStatus.Invited,
            InvitedAt = DateTime.UtcNow,
            InvitedByUserId = invitedByUserId
        };
        return user;
    }

    public void ActivateWithAuth0Id(string auth0UserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(auth0UserId);
        Auth0UserId = auth0UserId;
        Status = UserStatus.Active;
        SetUpdatedAt();
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        SetUpdatedAt();
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
        SetUpdatedAt();
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
        SetUpdatedAt();
    }

    public void Reactivate()
    {
        Status = UserStatus.Active;
        SetUpdatedAt();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}
