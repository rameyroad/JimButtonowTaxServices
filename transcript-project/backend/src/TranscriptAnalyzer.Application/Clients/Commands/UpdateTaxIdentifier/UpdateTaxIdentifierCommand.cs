using MediatR;
using TranscriptAnalyzer.Application.Clients.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Clients.Commands.UpdateTaxIdentifier;

public record UpdateTaxIdentifierCommand : IRequest<ClientDto?>
{
    public Guid Id { get; init; }
    public required string TaxIdentifier { get; init; }
    public required ClientType ClientType { get; init; }
    public required int Version { get; init; }
}
