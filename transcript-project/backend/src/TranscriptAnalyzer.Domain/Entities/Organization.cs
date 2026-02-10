using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Domain.Entities;

public class Organization : SoftDeletableEntity
{
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public Address Address { get; private set; }
    public SubscriptionStatus SubscriptionStatus { get; private set; }
    public string? SubscriptionPlan { get; private set; }
    public bool IsPlatformOrganization { get; private set; }

    private readonly List<User> _users = [];
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private readonly List<Client> _clients = [];
    public IReadOnlyCollection<Client> Clients => _clients.AsReadOnly();

    public OrganizationSettings? Settings { get; private set; }

#pragma warning disable CS8618 // Required for EF Core
    private Organization() { }
#pragma warning restore CS8618

    public Organization(
        string name,
        string slug,
        string contactEmail,
        Address address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactEmail);
        ArgumentNullException.ThrowIfNull(address);

        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        ContactEmail = contactEmail.Trim().ToLowerInvariant();
        Address = address;
        SubscriptionStatus = SubscriptionStatus.Trial;
    }

    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
        SetUpdatedAt();
    }

    public void UpdateContactInfo(string contactEmail, string? contactPhone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contactEmail);
        ContactEmail = contactEmail.Trim().ToLowerInvariant();
        ContactPhone = contactPhone?.Trim();
        SetUpdatedAt();
    }

    public void UpdateAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);
        Address = address;
        SetUpdatedAt();
    }

    public void UpdateSubscription(SubscriptionStatus status, string? plan = null)
    {
        SubscriptionStatus = status;
        SubscriptionPlan = plan;
        SetUpdatedAt();
    }

    public void SetSettings(OrganizationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Settings = settings;
    }

    public void SetPlatformOrganization(bool isPlatform)
    {
        IsPlatformOrganization = isPlatform;
        SetUpdatedAt();
    }
}
