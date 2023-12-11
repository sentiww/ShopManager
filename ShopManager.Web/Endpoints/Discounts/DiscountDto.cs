namespace ShopManager.Web.Endpoints.Discounts;

public sealed record DiscountDto
{
    public required Guid Id { get; init; }
    public required decimal Percentage { get; init; }
    public required DateTimeOffset StartDate { get; init; }
    public required DateTimeOffset EndDate { get; init; }
    public required Guid ProductId { get; init; }
}