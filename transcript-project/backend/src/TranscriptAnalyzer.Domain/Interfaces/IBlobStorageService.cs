namespace TranscriptAnalyzer.Domain.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(string containerName, string blobPath, Stream content, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string containerName, string blobPath, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string containerName, string blobPath, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string containerName, string blobPath, CancellationToken cancellationToken = default);
    Task<Uri> GetSasUriAsync(string containerName, string blobPath, TimeSpan validFor, CancellationToken cancellationToken = default);
}
