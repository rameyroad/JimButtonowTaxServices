using MediatR;
using TranscriptAnalyzer.Application.Invitations.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Invitations.Commands.CreateInvitation;

public record CreateInvitationCommand : IRequest<InvitationDto>
{
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
}
