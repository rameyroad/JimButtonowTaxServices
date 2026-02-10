using MediatR;

namespace TranscriptAnalyzer.Application.Invitations.Commands.AcceptInvitation;

public record AcceptInvitationCommand : IRequest<AcceptInvitationResult?>
{
    public required string Token { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}

public record AcceptInvitationResult
{
    public required Guid OrganizationId { get; init; }
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}
