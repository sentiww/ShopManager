using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopManager.Persistence.Entity;

namespace ShopManager.Persistence;

public class ShopManagerContext : IdentityDbContext<ShopManagerUserEntity>
{
    public DbSet<ShopManagerUserEntity> ShopManagerUsers { get; set; } = null!;
    public DbSet<ProductEntity> Products { get; set; } = null!;
    public DbSet<DiscountEntity> Discounts { get; set; } = null!;
    public DbSet<CollectionEntity> Collections { get; set; } = null!;

    public ShopManagerContext(DbContextOptions<ShopManagerContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}