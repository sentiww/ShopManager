using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Endpoints.Collections;

public sealed record AddCollectionRequest
{
    public required string Name { get; init; }
    public required Guid[] ProductIds { get; init; } = null!;
    public required string[] UserIds { get; init; } = null!;
}

public static class AddCollectionRequestExtensions
{
    public static CollectionEntity ToEntity(this AddCollectionRequest request) =>
        new ()
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };
}