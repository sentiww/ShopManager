namespace ShopManager.Client.Dtos;

public sealed record DiscountDto
{
    public required Guid Id { get; init; }
    public required decimal Percentage { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
}