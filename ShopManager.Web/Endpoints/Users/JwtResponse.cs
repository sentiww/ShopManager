namespace ShopManager.Web.Endpoints.Users;

public sealed record JwtResponse
{
    public required string Token { get; init; }
}