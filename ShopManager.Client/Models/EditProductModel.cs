using ShopManager.Common.Requests;

namespace ShopManager.Client.Models;

public sealed class EditProductModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int Quantity { get; set; }
}

public static class EditProductModelExtensions
{
    public static UpdateProductRequest ToRequest(this EditProductModel model) => new() 
    {
        Name = model.Name,
        Description = model.Description,
        Price = model.Price,
        Quantity = model.Quantity
    };
}