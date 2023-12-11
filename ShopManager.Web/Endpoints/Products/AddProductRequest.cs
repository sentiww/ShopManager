using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Endpoints.Products;

public sealed record AddProductRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int Quantity { get; init; }
}

public static class AddProductRequestExtensions
{
    public static ProductEntity ToEntity(this AddProductRequest request) =>
        new ()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity
        };
}