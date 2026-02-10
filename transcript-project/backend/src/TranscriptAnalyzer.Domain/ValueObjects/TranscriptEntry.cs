namespace TranscriptAnalyzer.Domain.ValueObjects;

public record TranscriptEntry
{
    public required string TransactionCode { get; init; }
    public required DateTime Date { get; init; }
    public decimal? Amount { get; init; }
    public required string Description { get; init; }
}
