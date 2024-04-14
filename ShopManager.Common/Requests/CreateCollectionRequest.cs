namespace ShopManager.Common.Requests;

public sealed record CreateCollectionRequest
{
    public required string Name { get; init; }
    public required Guid[] ProductIds { get; init; }
    public required string[] UserIds { get; init; }
}