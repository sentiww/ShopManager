namespace ShopManager.Client.Common;

public sealed record PaginatedResonse<TSource>
{
    public required IReadOnlyCollection<TSource> Items { get; init; } = null!;
    public required int TotalCount { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}