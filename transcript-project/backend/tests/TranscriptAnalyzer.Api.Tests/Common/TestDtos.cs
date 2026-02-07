using System.Text.Json;
using System.Text.Json.Serialization;

namespace TranscriptAnalyzer.Api.Tests.Common;

/// <summary>
/// Shared JSON serializer options for tests.
/// </summary>
public static class TestJsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
}

/// <summary>
/// Response type for deserializing paginated results in tests.
/// </summary>
public record PaginatedListResponse<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}
