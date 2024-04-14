using ShopManager.Common.Requests;

namespace ShopManager.Client.Models;

public sealed class AddProductModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int Quantity { get; set; }
}

public static class AddProductModelExtensions
{
    public static CreateProductRequest ToRequest(this AddProductModel model) => new() 
    {
        Name = model.Name,
        Description = model.Description,
        Price = model.Price,
        Quantity = model.Quantity
    };
}