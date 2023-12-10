using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopManager.Persistence;
using ShopManager.Persistence.Entity;
using ShopManager.Persistence.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopManagerContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ShopManagerUserEntity, IdentityRole>()
    .AddEntityFrameworkStores<ShopManagerContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

if (app.Environment.IsDevelopment() || true) // TODO: Remove true after testing
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await using var context = app.Services.GetRequiredService<ShopManagerContext>();
using var userManager = app.Services.GetRequiredService<UserManager<ShopManagerUserEntity>>();
await ShopManagerContextUtilities.TryMigrateAsync(context);
await ShopManagerContextUtilities.EnsureDefaultUserExistsAsync(context, userManager);

app.Run();