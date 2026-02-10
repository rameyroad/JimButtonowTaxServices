using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Application.Issues.Services;

public interface ITranscriptParser
{
    IReadOnlyList<TranscriptEntry> ParseTranscript(string content);
}
