namespace TranscriptAnalyzer.Domain.ValueObjects;

public sealed record EncryptedString
{
    public string EncryptedValue { get; }

    private EncryptedString()
    {
        EncryptedValue = string.Empty;
    }

    private EncryptedString(string encryptedValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(encryptedValue);
        EncryptedValue = encryptedValue;
    }

    public static EncryptedString FromEncrypted(string encryptedValue)
    {
        return new EncryptedString(encryptedValue);
    }

    public static EncryptedString Empty => new();

    public bool IsEmpty => string.IsNullOrEmpty(EncryptedValue);

    public override string ToString() => "[ENCRYPTED]";
}
