using ShopManager.Client.Requests;

namespace ShopManager.Client.Models;

public sealed class RegisterModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public static class RegisterModelExtensions
{
    public static RegisterRequest ToRequest(this RegisterModel model) =>
        new()
        {
            UserName = model.UserName,
            Email = model.Email,
            Password = model.Password
        };
}