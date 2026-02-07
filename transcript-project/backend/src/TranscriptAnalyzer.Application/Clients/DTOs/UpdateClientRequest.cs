using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.DTOs;

public record UpdateClientRequest
{
    // Individual fields
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    // Business fields
    public string? BusinessName { get; init; }
    public BusinessEntityType? EntityType { get; init; }
    public string? ResponsibleParty { get; init; }

    // Common fields (tax identifier requires re-encryption)
    public string? TaxIdentifier { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public AddressDto? Address { get; init; }
    public string? Notes { get; init; }

    // Required for optimistic concurrency
    public required int Version { get; init; }
}
