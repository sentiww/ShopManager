using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Endpoints.Products;

public sealed record UpdateProductRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int Quantity { get; init; }
}

public static class UpdateProductRequestExtensions
{
    public static void UpdateEntity(this UpdateProductRequest request, ProductEntity entity)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Quantity = request.Quantity;
    }
}