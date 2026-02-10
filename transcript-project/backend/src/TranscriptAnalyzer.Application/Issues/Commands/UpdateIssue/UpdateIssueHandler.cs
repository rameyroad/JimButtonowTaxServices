using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Common;
using TranscriptAnalyzer.Application.Issues.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Issues.Commands.UpdateIssue;

public class UpdateIssueHandler : IRequestHandler<UpdateIssueCommand, IssueDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private readonly IMapper _mapper;

    public UpdateIssueHandler(DbContext dbContext, ITenantContext tenantContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
        _mapper = mapper;
    }

    public async Task<IssueDetailDto?> Handle(
        UpdateIssueCommand request,
        CancellationToken cancellationToken)
    {
        var issue = await _dbContext.Set<Issue>()
            .FirstOrDefaultAsync(i => i.Id == request.IssueId && i.ClientId == request.ClientId,
                cancellationToken);

        if (issue is null)
            return null;

        if (request.Severity.HasValue)
            issue.UpdateSeverity(request.Severity.Value);

        if (request.Description is not null)
            issue.UpdateDescription(request.Description);

        if (request.Amount.HasValue)
            issue.UpdateAmount(request.Amount);

        if (request.Resolve.HasValue)
        {
            if (request.Resolve.Value)
            {
                var userId = _tenantContext.UserId
                    ?? throw new UnauthorizedAccessException("User context is required.");
                issue.Resolve(userId);
            }
            else
            {
                issue.Unresolve();
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<IssueDetailDto>(issue);
    }
}
