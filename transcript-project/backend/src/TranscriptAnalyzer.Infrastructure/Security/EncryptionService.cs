using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using TranscriptAnalyzer.Domain.Interfaces;
using TranscriptAnalyzer.Domain.ValueObjects;

namespace TranscriptAnalyzer.Infrastructure.Security;

public sealed class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration)
    {
        var keyBase64 = configuration["Encryption:Key"]
            ?? throw new InvalidOperationException("Encryption:Key configuration is required.");

        _key = Convert.FromBase64String(keyBase64);

        if (_key.Length != 32)
        {
            throw new InvalidOperationException("Encryption key must be 256 bits (32 bytes).");
        }
    }

    public EncryptedString Encrypt(string plainText)
    {
        ArgumentException.ThrowIfNullOrEmpty(plainText);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Prepend IV to encrypted data
        var result = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

        return EncryptedString.FromEncrypted(Convert.ToBase64String(result));
    }

    public string Decrypt(EncryptedString encryptedString)
    {
        ArgumentNullException.ThrowIfNull(encryptedString);

        if (encryptedString.IsEmpty)
        {
            return string.Empty;
        }

        var fullCipher = Convert.FromBase64String(encryptedString.EncryptedValue);

        using var aes = Aes.Create();
        aes.Key = _key;

        // Extract IV from the beginning of the cipher
        var iv = new byte[aes.BlockSize / 8];
        var cipher = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    public string ComputeHash(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(value + Convert.ToBase64String(_key)));
        return Convert.ToBase64String(hashBytes);
    }
}
