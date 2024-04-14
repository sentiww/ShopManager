namespace ShopManager.Common.Requests;

public sealed record UpdateDiscountRequest
{
    public required decimal Percentage { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required Guid ProductId { get; init; }
}