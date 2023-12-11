using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Endpoints.Discounts;

public sealed record CreateDiscountRequest
{
    public required Guid ProductId { get; init; }
    public required decimal Percentage { get; init; }
    public required DateTimeOffset StartDate { get; init; }
    public required DateTimeOffset EndDate { get; init; }
}

public static class CreateDiscountRequestExtensions
{
    public static DiscountEntity ToEntity(this CreateDiscountRequest request) =>
        new ()
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Percentage = request.Percentage,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
}