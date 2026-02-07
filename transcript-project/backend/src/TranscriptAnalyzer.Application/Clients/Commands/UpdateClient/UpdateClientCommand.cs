using MediatR;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand : IRequest<ClientDto?>
{
    public Guid Id { get; init; }

    // Individual fields
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    // Business fields
    public string? BusinessName { get; init; }
    public BusinessEntityType? EntityType { get; init; }
    public string? ResponsibleParty { get; init; }

    // Common fields
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public AddressDto? Address { get; init; }
    public string? Notes { get; init; }

    // Required for optimistic concurrency
    public required int Version { get; init; }
}
