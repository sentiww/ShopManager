using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopManager.Persistence.Entity;

namespace ShopManager.Persistence.Utilities;

public static class ShopManagerContextUtilities
{
    private const string DefaultAdminUsername = "admin";
    
    public static async Task MigrateOrThrowAsync(ShopManagerContext context)
    {
        var canConnect = await context.Database.CanConnectAsync();

        if (canConnect is false)
        {
            throw new Exception("Cannot connect to database");    
        }
            
        await context.Database.MigrateAsync();
    }

    public static async Task EnsureDefaultUserExistsAsync(ShopManagerContext context, UserManager<ShopManagerUserEntity> userManager)
    {
        const string defaultAdminEmail = "contact@senti.dev";
        const string defaultAdminPassword = "Admin123!";
        
        var adminExists = await context.Users
            .AnyAsync(user => user.UserName == DefaultAdminUsername);

        if (adminExists is false)
        {
            await userManager.CreateAsync(new ShopManagerUserEntity
            { 
                UserName = DefaultAdminUsername,
                Email = defaultAdminEmail,
                EmailConfirmed = true
            }, defaultAdminPassword);
        }
        
        await context.SaveChangesAsync();
    }

    public static async Task AddFakeEntitiesAsync(ShopManagerContext context)
    {
        // Admin user
        var adminUser = await context.Users
            .FirstAsync(user => user.UserName == DefaultAdminUsername);
        
        // Audio items
        var headphones = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "DT 770 PRO",
            Description = "DT 770 PRO, 250 ohms: for mixing applications in the studio",
            Price = 299.99M,
            Quantity = 10
        };
        var microphone = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "AT2020",
            Description = "The price/performance standard in side-address studio condenser microphone technology",
            Price = 149.99M,
            Quantity = 10
        };
        var amplifier = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "Scarlett 2i2",
            Description = "The best selling USB audio interface in the world",
            Price = 159.99M,
            Quantity = 10
        };
        
        // Video items
        var camera = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "EOS 5D Mark IV",
            Description = "30.4 Megapixel full-frame CMOS sensor for versatile shooting in nearly any light",
            Price = 2499.99M,
            Quantity = 10
        };
        var lens = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "EF 24-70mm f/2.8L II USM",
            Description = "The EF 24-70mm f/2.8L II USM’s constant f/2.8 aperture throughout the zoom range makes the lens a great low light performer",
            Price = 1899.99M,
            Quantity = 10
        };
        var tripod = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "Manfrotto 190XPRO",
            Description = "The 190XPRO Aluminium 4-Section Tripod with Horizontal Column is more than just an average photo stand",
            Price = 199.99M,
            Quantity = 10
        };
        
        // Computer items
        var cpu = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "AMD Ryzen 9 5950X",
            Description = "The Fastest in the Game",
            Price = 799.99M,
            Quantity = 10
        };
        var gpu = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "NVIDIA GeForce RTX 3090",
            Description = "GeForce RTX™ 3090 GAMING X TRIO 24G",
            Price = 1499.99M,
            Quantity = 10
        };
        var ram = new ProductEntity
        {
            Id = Guid.NewGuid(),
            Name = "Corsair Vengeance RGB Pro",
            Description = "CORSAIR VENGEANCE RGB PRO Series DDR4 overclocked memory lights up your PC with mesmerizing dynamic multi-zone RGB lighting",
            Price = 199.99M,
            Quantity = 10
        };
        
        // Audio collection
        var audioCollection = new CollectionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Audio",
            Products = new List<ProductEntity>
            {
                headphones,
                microphone,
                amplifier
            },
            Users = new List<ShopManagerUserEntity>
            {
                adminUser
            }
        };
        
        // Video collection
        var videoCollection = new CollectionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Video",
            Products = new List<ProductEntity>
            {
                camera,
                lens,
                tripod
            }
        };
        
        // Computer collection
        var computerCollection = new CollectionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Computer",
            Products = new List<ProductEntity>
            {
                cpu,
                gpu,
                ram
            }
        };
        
        // Discounts
        var audioDiscount = new DiscountEntity
        {
            Id = Guid.NewGuid(),
            Percentage = 10,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            ProductId = headphones.Id
        };
        var videoDiscount = new DiscountEntity
        {
            Id = Guid.NewGuid(),
            Percentage = 10,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            ProductId = tripod.Id
        };
        
        context.Collections.Add(audioCollection);
        context.Collections.Add(videoCollection);
        context.Collections.Add(computerCollection);
        context.Discounts.Add(audioDiscount);
        context.Discounts.Add(videoDiscount);
        
        await context.SaveChangesAsync();
    }

    public static async Task RemoveAllEntitiesAsync(ShopManagerContext context)
    {
        context.RemoveRange(context.Collections);
        context.RemoveRange(context.Discounts);
        context.RemoveRange(context.Products);
        await context.SaveChangesAsync();
    }
}