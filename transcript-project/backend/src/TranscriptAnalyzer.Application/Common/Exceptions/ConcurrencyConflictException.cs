namespace TranscriptAnalyzer.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an optimistic concurrency conflict occurs.
/// This happens when trying to update an entity with a stale version.
/// </summary>
public sealed class ConcurrencyConflictException : Exception
{
    public ConcurrencyConflictException()
    {
    }

    public ConcurrencyConflictException(string message) : base(message)
    {
    }

    public ConcurrencyConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
