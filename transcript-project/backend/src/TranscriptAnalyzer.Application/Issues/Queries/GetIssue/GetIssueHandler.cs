using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TranscriptAnalyzer.Application.Issues.DTOs;
using TranscriptAnalyzer.Domain.Entities;

namespace TranscriptAnalyzer.Application.Issues.Queries.GetIssue;

public class GetIssueHandler : IRequestHandler<GetIssueQuery, IssueDetailDto?>
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;

    public GetIssueHandler(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IssueDetailDto?> Handle(
        GetIssueQuery request,
        CancellationToken cancellationToken)
    {
        var issue = await _dbContext.Set<Issue>()
            .FirstOrDefaultAsync(i => i.Id == request.IssueId && i.ClientId == request.ClientId,
                cancellationToken);

        if (issue is null)
            return null;

        return _mapper.Map<IssueDetailDto>(issue);
    }
}
