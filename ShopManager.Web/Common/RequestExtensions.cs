using ShopManager.Common.Requests;
using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Common;

public static class RequestExtensions
{
    public static ProductEntity ToEntity(this CreateProductRequest request) =>
        new ()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity
        };
    
    public static void UpdateEntity(this UpdateProductRequest request, ProductEntity entity)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Quantity = request.Quantity;
    }
    
    public static CollectionEntity ToEntity(this CreateCollectionRequest request) =>
        new ()
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };
    
    public static void UpdateEntity(this UpdateCollectionRequest request, CollectionEntity collection)
    {
        collection.Name = request.Name;
    }
    
    public static DiscountEntity ToEntity(this CreateDiscountRequest request) =>
        new ()
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Percentage = request.Percentage,
            StartDate = request.StartDate.ToUniversalTime(),
            EndDate = request.EndDate.ToUniversalTime()
        };
    
    public static void UpdateEntity(this UpdateDiscountRequest request, DiscountEntity entity)
    {
        entity.Percentage = request.Percentage;
        entity.StartDate = request.StartDate.ToUniversalTime();
        entity.EndDate = request.EndDate.ToUniversalTime();
    }
}