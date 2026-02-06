using TranscriptAnalyzer.Domain.Common;
using TranscriptAnalyzer.Domain.Enums;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Domain.Entities;

public class Client : TenantEntity
{
    public ClientType ClientType { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? BusinessName { get; private set; }
    public BusinessEntityType? EntityType { get; private set; }
    public string? ResponsibleParty { get; private set; }
    public EncryptedString TaxIdentifier { get; private set; }
    public string TaxIdentifierLast4 { get; private set; }
    public string Email { get; private set; }
    public string? Phone { get; private set; }
    public Address Address { get; private set; }
    public string? Notes { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    public Organization? Organization { get; private set; }
    public User? CreatedBy { get; private set; }

    private readonly List<Authorization> _authorizations = [];
    public IReadOnlyCollection<Authorization> Authorizations => _authorizations.AsReadOnly();

    private readonly List<Transcript> _transcripts = [];
    public IReadOnlyCollection<Transcript> Transcripts => _transcripts.AsReadOnly();

    public string DisplayName => ClientType == ClientType.Individual
        ? $"{LastName}, {FirstName}"
        : BusinessName ?? string.Empty;

#pragma warning disable CS8618 // Required for EF Core
    private Client() { }
#pragma warning restore CS8618

    public static Client CreateIndividual(
        Guid organizationId,
        string firstName,
        string lastName,
        EncryptedString taxIdentifier,
        string taxIdentifierLast4,
        string email,
        Address address,
        Guid createdByUserId,
        string? phone = null,
        string? notes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentNullException.ThrowIfNull(taxIdentifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(taxIdentifierLast4);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(address);

        return new Client
        {
            OrganizationId = organizationId,
            ClientType = ClientType.Individual,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            TaxIdentifier = taxIdentifier,
            TaxIdentifierLast4 = taxIdentifierLast4,
            Email = email.Trim().ToLowerInvariant(),
            Phone = phone?.Trim(),
            Address = address,
            Notes = notes?.Trim(),
            CreatedByUserId = createdByUserId
        };
    }

    public static Client CreateBusiness(
        Guid organizationId,
        string businessName,
        BusinessEntityType entityType,
        EncryptedString taxIdentifier,
        string taxIdentifierLast4,
        string email,
        Address address,
        Guid createdByUserId,
        string? responsibleParty = null,
        string? phone = null,
        string? notes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(businessName);
        ArgumentNullException.ThrowIfNull(taxIdentifier);
        ArgumentException.ThrowIfNullOrWhiteSpace(taxIdentifierLast4);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentNullException.ThrowIfNull(address);

        return new Client
        {
            OrganizationId = organizationId,
            ClientType = ClientType.Business,
            BusinessName = businessName.Trim(),
            EntityType = entityType,
            ResponsibleParty = responsibleParty?.Trim(),
            TaxIdentifier = taxIdentifier,
            TaxIdentifierLast4 = taxIdentifierLast4,
            Email = email.Trim().ToLowerInvariant(),
            Phone = phone?.Trim(),
            Address = address,
            Notes = notes?.Trim(),
            CreatedByUserId = createdByUserId
        };
    }

    public void UpdateContactInfo(string email, string? phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
        SetUpdatedAt();
    }

    public void UpdateAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);
        Address = address;
        SetUpdatedAt();
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
        SetUpdatedAt();
    }

    public void UpdateIndividualName(string firstName, string lastName)
    {
        if (ClientType != ClientType.Individual)
        {
            throw new InvalidOperationException("Cannot update individual name on a business client.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        SetUpdatedAt();
    }

    public void UpdateBusinessInfo(string businessName, BusinessEntityType entityType, string? responsibleParty)
    {
        if (ClientType != ClientType.Business)
        {
            throw new InvalidOperationException("Cannot update business info on an individual client.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(businessName);

        BusinessName = businessName.Trim();
        EntityType = entityType;
        ResponsibleParty = responsibleParty?.Trim();
        SetUpdatedAt();
    }
}
