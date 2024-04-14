using Microsoft.EntityFrameworkCore;

namespace ShopManager.Common.Utilities;

public sealed class PagedCollection<TSource>
{
    public required IReadOnlyCollection<TSource> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}

public static class PagedCollectionExtensions
{
    public static async Task<PagedCollection<TSource>> ToPagedCollectionAsync<TSource>(
        this IQueryable<TSource> source,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), page, "Page must be greater than or equal to 0.");
        }
        
        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be greater than or equal to 1.");
        }
        
        var skip = page * pageSize;
        
        var totalCount = await source.CountAsync(ct);
        
        var items = await source
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedCollection<TSource>
        {
            Items = items.AsReadOnly(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}