using MediatR;
using TranscriptAnalyzer.Application.Invitations.DTOs;

namespace TranscriptAnalyzer.Application.Invitations.Queries.ListInvitations;

public record ListInvitationsQuery : IRequest<IReadOnlyList<InvitationDto>>;
