using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Invitations.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Invitations.Queries.ListInvitations;

public class ListInvitationsHandler : IRequestHandler<ListInvitationsQuery, IReadOnlyList<InvitationDto>>
{
    private readonly DbContext _dbContext;

    public ListInvitationsHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<InvitationDto>> Handle(
        ListInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Invitation>()
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvitationDto
            {
                Id = i.Id,
                Email = i.Email,
                Role = i.Role,
                Status = i.Status,
                ExpiresAt = i.ExpiresAt,
                AcceptedAt = i.AcceptedAt,
                InvitedByUserId = i.InvitedByUserId,
                CreatedAt = i.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
