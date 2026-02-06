using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using TranscriptAnalyzer.Domain.Interfaces;

namespace TranscriptAnalyzer.Infrastructure.Storage;

public sealed class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureStorage:ConnectionString"]
            ?? throw new InvalidOperationException("AzureStorage:ConnectionString configuration is required.");

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadAsync(
        string containerName,
        string blobPath,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobPath);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobPath);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };

        await blobClient.UploadAsync(content, options, cancellationToken);

        return blobPath;
    }

    public async Task<Stream> DownloadAsync(
        string containerName,
        string blobPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobPath);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return response.Value.Content;
    }

    public async Task<bool> DeleteAsync(
        string containerName,
        string blobPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobPath);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public async Task<bool> ExistsAsync(
        string containerName,
        string blobPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobPath);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        var response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }

    public Task<Uri> GetSasUriAsync(
        string containerName,
        string blobPath,
        TimeSpan validFor,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobPath);

        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException("BlobClient cannot generate SAS URI. Ensure the connection uses account key credentials.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobPath,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);

        return Task.FromResult(sasUri);
    }
}
