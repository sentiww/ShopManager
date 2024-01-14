namespace ShopManager.Client.Dtos;

public sealed record JwtDto
{
    public required string Token { get; init; }
}