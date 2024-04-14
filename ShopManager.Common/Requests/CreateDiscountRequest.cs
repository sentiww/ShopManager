namespace ShopManager.Common.Requests;

public sealed record CreateDiscountRequest
{
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid ProductId { get; set; }
}