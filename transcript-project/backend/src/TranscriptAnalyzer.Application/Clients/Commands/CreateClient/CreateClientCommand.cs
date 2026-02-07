using MediatR;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.Commands.CreateClient;

public record CreateClientCommand : IRequest<ClientDto>
{
    public required ClientType ClientType { get; init; }

    // Individual fields
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    // Business fields
    public string? BusinessName { get; init; }
    public BusinessEntityType? EntityType { get; init; }
    public string? ResponsibleParty { get; init; }

    // Common fields
    public required string TaxIdentifier { get; init; }
    public required string Email { get; init; }
    public string? Phone { get; init; }
    public required AddressDto Address { get; init; }
    public string? Notes { get; init; }
}
