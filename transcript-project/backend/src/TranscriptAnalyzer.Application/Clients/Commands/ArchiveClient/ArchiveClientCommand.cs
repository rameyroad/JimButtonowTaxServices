using MediatR;

namespace TranscriptAnalyzer.Application.Clients.Commands.ArchiveClient;

public record ArchiveClientCommand(Guid Id) : IRequest<bool>;
