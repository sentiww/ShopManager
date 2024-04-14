namespace ShopManager.Common.Requests;

public sealed record RegisterRequest
{
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}