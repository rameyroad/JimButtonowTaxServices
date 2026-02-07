using MediatR;
using TranscriptAnalyzer.Application.Clients.DTOs;

namespace TranscriptAnalyzer.Application.Clients.Queries.GetClient;

public record GetClientQuery(Guid Id) : IRequest<ClientDetailDto?>;
