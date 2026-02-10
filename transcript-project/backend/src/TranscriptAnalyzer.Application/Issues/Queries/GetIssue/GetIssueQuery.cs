using MediatR;
using TranscriptAnalyzer.Application.Issues.DTOs;

namespace TranscriptAnalyzer.Application.Issues.Queries.GetIssue;

public record GetIssueQuery(Guid ClientId, Guid IssueId) : IRequest<IssueDetailDto?>;
