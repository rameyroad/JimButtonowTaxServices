using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.DTOs;

public record ClientDto
{
    public required Guid Id { get; init; }
    public required ClientType ClientType { get; init; }
    public required string DisplayName { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? BusinessName { get; init; }
    public BusinessEntityType? EntityType { get; init; }
    public string? ResponsibleParty { get; init; }
    public required string TaxIdentifierLast4 { get; init; }
    public required string TaxIdentifierMasked { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public required AddressDto Address { get; init; }
    public bool IsArchived { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required int Version { get; init; }
}
