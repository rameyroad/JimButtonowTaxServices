using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Domain.Interfaces;

public interface IEncryptionService
{
    EncryptedString Encrypt(string plainText);
    string Decrypt(EncryptedString encryptedString);
    string ComputeHash(string value);
}
