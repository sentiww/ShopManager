using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ShopManager.Persistence;
using ShopManager.Persistence.Entity;
using ShopManager.Persistence.Utilities;
using ShopManager.Web.Endpoints;
using ShopManager.Web.Endpoints.Collections;
using ShopManager.Web.Endpoints.Discounts;
using ShopManager.Web.Endpoints.Products;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopManagerContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    var dataSource = dataSourceBuilder.Build();
    options.UseNpgsql(dataSource);
});

builder.Services.AddIdentity<ShopManagerUserEntity, IdentityRole>()
    .AddEntityFrameworkStores<ShopManagerContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddApiVersioning()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

using var scope = app.Services.CreateScope();
await using var context = scope.ServiceProvider.GetRequiredService<ShopManagerContext>();
using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShopManagerUserEntity>>();
await ShopManagerContextUtilities.MigrateOrThrowAsync(context);
await ShopManagerContextUtilities.EnsureDefaultUserExistsAsync(context, userManager);
await ShopManagerContextUtilities.RemoveAllEntitiesAsync(context);
await ShopManagerContextUtilities.AddFakeEntitiesAsync(context);

if (app.Environment.IsDevelopment() || true) // TODO: Remove true after testing
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(1)
    .Build();

app.MapProductEndpoints(versionSet);
app.MapCollectionEndpoints(versionSet);
app.MapDiscountEndpoints(versionSet);

app.Run();