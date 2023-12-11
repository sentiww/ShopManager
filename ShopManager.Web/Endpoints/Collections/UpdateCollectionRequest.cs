using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Endpoints.Collections;

public sealed record UpdateCollectionRequest
{
    public required string Name { get; init; }
    public required Guid[] ProductIds { get; init; } = null!;
    public required string[] UserIds { get; init; } = null!;
}

public static class UpdateCollectionRequestExtensions
{
    public static void UpdateEntity(this UpdateCollectionRequest request, CollectionEntity collection)
    {
        collection.Name = request.Name;
    }
}