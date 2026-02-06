using Microsoft.EntityFrameworkCore;

namespace TranscriptAnalyzer.Application.Common;

public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PaginatedList(IReadOnlyList<T> items, int count, int page, int pageSize)
    {
        Items = items;
        TotalCount = count;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, count, page, pageSize);
    }

    public static PaginatedList<T> Create(
        IEnumerable<T> source,
        int page,
        int pageSize)
    {
        var sourceList = source.ToList();
        var count = sourceList.Count;
        var items = sourceList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<T>(items, count, page, pageSize);
    }

    public PaginatedList<TDestination> Map<TDestination>(Func<T, TDestination> selector)
    {
        var mappedItems = Items.Select(selector).ToList();
        return new PaginatedList<TDestination>(mappedItems, TotalCount, Page, PageSize);
    }
}

public record PaginationRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public static int MaxPageSize => 100;

    public PaginationRequest Normalize()
    {
        return this with
        {
            Page = Math.Max(1, Page),
            PageSize = Math.Clamp(PageSize, 1, MaxPageSize)
        };
    }
}
