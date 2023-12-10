using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopManager.Persistence.Entity;

namespace ShopManager.Persistence;

public class ShopManagerContext : IdentityDbContext<ShopManagerUserEntity>
{
    public ShopManagerContext(DbContextOptions<ShopManagerContext> options) : base(options) { }
}