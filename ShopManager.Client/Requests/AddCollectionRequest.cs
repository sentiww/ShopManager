namespace ShopManager.Client.Requests;

public sealed record AddCollectionRequest
{
    public required string Name { get; init; }
    public required Guid[] ProductIds { get; init; }
    public required string[] UserIds { get; init; }
}