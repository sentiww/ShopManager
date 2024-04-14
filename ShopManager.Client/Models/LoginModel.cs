using ShopManager.Common.Requests;

namespace ShopManager.Client.Models;

public sealed class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public static class LoginModelExtensions
{
    public static LoginRequest ToRequest(this LoginModel model) =>
        new()
        {
            Email = model.Email,
            Password = model.Password
        };
}