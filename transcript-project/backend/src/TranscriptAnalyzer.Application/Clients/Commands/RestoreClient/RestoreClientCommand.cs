using MediatR;
using TranscriptAnalyzer.Application.Clients.DTOs;

namespace TranscriptAnalyzer.Application.Clients.Commands.RestoreClient;

/// <summary>
/// Result of a restore operation.
/// </summary>
public record RestoreClientResult(bool Found, bool WasArchived, ClientDto? Client);

public record RestoreClientCommand(Guid Id) : IRequest<RestoreClientResult>;
