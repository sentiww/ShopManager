namespace ShopManager.Client.Requests;

public sealed record AddDiscountRequest
{
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid ProductId { get; set; }
}