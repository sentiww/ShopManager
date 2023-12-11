using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Endpoints.Discounts;

public sealed record UpdateDiscountRequest
{
    public required Guid ProductId { get; init; }
    public decimal Percentage { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
}

public static class UpdateDiscountRequestExtensions
{
    public static void UpdateEntity(this UpdateDiscountRequest request, DiscountEntity entity)
    {
        entity.Percentage = request.Percentage;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
    }
}