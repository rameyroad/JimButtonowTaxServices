using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.ClientApprovals.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.ClientApprovals.Queries.GetApprovalByToken;

public class GetApprovalByTokenHandler : IRequestHandler<GetApprovalByTokenQuery, ClientApprovalDto?>
{
    private readonly DbContext _dbContext;

    public GetApprovalByTokenHandler(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClientApprovalDto?> Handle(
        GetApprovalByTokenQuery request,
        CancellationToken cancellationToken)
    {
        // Bypass tenant filter — this is a public endpoint accessed via token
        var approval = await _dbContext.Set<ClientApproval>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.Token == request.Token && a.DeletedAt == null, cancellationToken);

        if (approval is null)
            return null;

        return new ClientApprovalDto
        {
            Id = approval.Id,
            Title = approval.Title,
            Description = approval.Description,
            Status = approval.Status,
            TokenExpiresAt = approval.TokenExpiresAt,
            RespondedAt = approval.RespondedAt,
            ResponseNotes = approval.ResponseNotes,
            CreatedAt = approval.CreatedAt
        };
    }
}
