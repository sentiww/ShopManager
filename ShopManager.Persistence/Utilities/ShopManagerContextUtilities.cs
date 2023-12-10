using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopManager.Persistence.Entity;

namespace ShopManager.Persistence.Utilities;

public static class ShopManagerContextUtilities
{
    public static async Task<bool> TryMigrateAsync(ShopManagerContext context)
    {
        var canConnect = await context.Database.CanConnectAsync();

        if (canConnect)
        {
            await context.Database.MigrateAsync();
        }
        
        return canConnect;
    }

    public static async Task EnsureDefaultUserExistsAsync(ShopManagerContext context, UserManager<ShopManagerUserEntity> userManager)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.UserName == "admin");

        if (user is null)
        {
            await userManager.CreateAsync(new ShopManagerUserEntity
            {
                UserName = "admin",
                Email = "contact@senti.dev",
                EmailConfirmed = true
            }, "Admin123!");
        }
        
        await context.SaveChangesAsync();
    }
}