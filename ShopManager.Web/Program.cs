using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ShopManager.Persistence;
using ShopManager.Persistence.Entity;
using ShopManager.Persistence.Utilities;
using ShopManager.Web.Endpoints.Collections;
using ShopManager.Web.Endpoints.Discounts;
using ShopManager.Web.Endpoints.Products;
using ShopManager.Web.Endpoints.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
var dataSource = dataSourceBuilder.Build();
builder.Services.AddDbContext<ShopManagerContext>(options =>
{
    options.UseNpgsql(dataSource);
});

builder.Services.AddIdentity<ShopManagerUserEntity, IdentityRole>()
    .AddEntityFrameworkStores<ShopManagerContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddJwtBearer();
builder.Services.AddAuthorization();

var corsPolicyName = "DefaultPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services
    .AddApiVersioning()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
await using var context = scope.ServiceProvider.GetRequiredService<ShopManagerContext>();
using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShopManagerUserEntity>>();
await ShopManagerContextUtilities.CheckConnectionAsync(context);
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
app.MapUserEndpoints(versionSet);

app.Run();

public partial class Program { }