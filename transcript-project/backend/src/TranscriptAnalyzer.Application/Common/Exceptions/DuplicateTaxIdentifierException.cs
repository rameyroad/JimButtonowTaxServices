namespace TranscriptAnalyzer.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a client with a tax identifier
/// that already exists within the same organization/tenant.
/// </summary>
public sealed class DuplicateTaxIdentifierException : Exception
{
    public DuplicateTaxIdentifierException()
    {
    }

    public DuplicateTaxIdentifierException(string message) : base(message)
    {
    }

    public DuplicateTaxIdentifierException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
