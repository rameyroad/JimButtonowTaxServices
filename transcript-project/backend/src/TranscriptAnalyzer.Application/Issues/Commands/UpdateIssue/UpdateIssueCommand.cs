using MediatR;
using TranscriptAnalyzer.Application.Issues.DTOs;
using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Issues.Commands.UpdateIssue;

public record UpdateIssueCommand : IRequest<IssueDetailDto?>
{
    public Guid ClientId { get; init; }
    public Guid IssueId { get; init; }
    public IssueSeverity? Severity { get; init; }
    public string? Description { get; init; }
    public decimal? Amount { get; init; }
    public bool? Resolve { get; init; }
}
