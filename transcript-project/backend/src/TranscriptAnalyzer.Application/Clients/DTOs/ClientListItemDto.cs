using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.DTOs;

public record ClientListItemDto
{
    public required Guid Id { get; init; }
    public required ClientType ClientType { get; init; }
    public required string DisplayName { get; init; }
    public required string TaxIdentifierLast4 { get; init; }
    public required string TaxIdentifierMasked { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public bool IsArchived { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
