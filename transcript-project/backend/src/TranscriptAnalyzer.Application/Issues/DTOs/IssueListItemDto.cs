using TranscriptAnalyzer.Domain.Enums;

namespace TranscriptAnalyzer.Application.Issues.DTOs;

public record IssueListItemDto
{
    public required Guid Id { get; init; }
    public required Guid ClientId { get; init; }
    public required IssueType IssueType { get; init; }
    public required IssueSeverity Severity { get; init; }
    public required int TaxYear { get; init; }
    public decimal? Amount { get; init; }
    public required string Description { get; init; }
    public string? TransactionCode { get; init; }
    public required DateTime DetectedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public required DateTime CreatedAt { get; init; }
}
