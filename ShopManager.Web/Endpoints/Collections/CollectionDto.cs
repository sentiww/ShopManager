namespace ShopManager.Web.Endpoints.Collections;

public sealed record CollectionDto
{
    public sealed record CollectionProductDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }

    public sealed record CollectionUserDto
    {
        public required string Id { get; init; }
        public required string? UserName { get; init; }
    }

    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required IReadOnlyCollection<CollectionProductDto> Products { get; init; } = null!;
    public required IReadOnlyCollection<CollectionUserDto> Users { get; init; } = null!;
}