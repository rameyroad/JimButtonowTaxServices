using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationHandler : IRequestHandler<AcceptInvitationCommand, AcceptInvitationResult?>
{
    private readonly DbContext _dbContext;

    public AcceptInvitationHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AcceptInvitationResult?> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken)
    {
        // Find invitation by token — bypass tenant filter since we don't know the org yet
        var invitation = await _dbContext.Set<Invitation>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Token == request.Token && i.DeletedAt == null, cancellationToken);

        if (invitation is null || !invitation.CanAccept)
            return null;

        // Accept the invitation
        invitation.Accept();

        // Create the user
        var user = User.CreateInvitedUser(
            invitation.OrganizationId,
            invitation.Email,
            request.FirstName,
            request.LastName,
            invitation.Role,
            invitation.InvitedByUserId);

        _dbContext.Set<User>().Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AcceptInvitationResult
        {
            OrganizationId = invitation.OrganizationId,
            UserId = user.Id,
            Email = invitation.Email
        };
    }
}
