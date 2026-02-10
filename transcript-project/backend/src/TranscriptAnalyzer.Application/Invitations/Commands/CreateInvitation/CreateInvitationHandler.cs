using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Invitations.DTOs;
using TranscriptAnalyzer.Domain.Entities;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Invitations.Commands.CreateInvitation;

public class CreateInvitationHandler : IRequestHandler<CreateInvitationCommand, InvitationDto>
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public CreateInvitationHandler(DbContext dbContext, ITenantContext tenantContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<InvitationDto> Handle(
        CreateInvitationCommand request,
        CancellationToken cancellationToken)
    {
        var orgId = _tenantContext.OrganizationId
            ?? throw new InvalidOperationException("Organization context required.");

        var userId = _tenantContext.UserId
            ?? throw new InvalidOperationException("User context required.");

        // Check if there's already a pending invitation for this email
        // Invitation entity stores email as lowercase, so compare directly
        var emailToCheck = request.Email.Trim();
        var existing = await _dbContext.Set<Invitation>()
            .FirstOrDefaultAsync(i =>
                i.Email == emailToCheck &&
                i.Status == InvitationStatus.Pending,
                cancellationToken);

        if (existing is not null)
        {
            throw new InvalidOperationException($"A pending invitation already exists for {request.Email}.");
        }

        var invitation = new Invitation(
            orgId,
            request.Email,
            request.Role,
            userId);

        _dbContext.Set<Invitation>().Add(invitation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<InvitationDto>(invitation);
    }
}
