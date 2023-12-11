using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Persistence;
using ShopManager.Web.Utilities;

namespace ShopManager.Web.Endpoints.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(
        this IEndpointRouteBuilder builder,
        ApiVersionSet versionSet)
    {
        var group = builder.MapGroup("api/v{apiVersion:apiVersion}/products");

        group.MapGet("", GetProducts)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapGet("{id}", GetProduct)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapPost("", CreateProduct)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapPut("{id}", UpdateProduct)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapDelete("{id}", DeleteProduct)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        
        return builder;
    }

    private static async Task<Ok<PagedCollection<ProductDto>>> GetProducts(
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var products = await context.Products
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            })
            .OrderBy(product => product.Id)
            .ToPagedCollectionAsync(1, 10, ct);

        return TypedResults.Ok(products);
    }

    private static async Task<Results<BadRequest<string>, Ok<ProductDto>>> GetProduct(
        [FromRoute] Guid id,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var product = await context.Products
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            })
            .FirstOrDefaultAsync(product => product.Id == id, ct);
        
        if (product is null)
        {
            return TypedResults.BadRequest<string>("Product not found");
        }
        
        return TypedResults.Ok(product);
    }

    private static async Task<Ok<Guid>> CreateProduct(
        [FromBody] AddProductRequest request,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var product = request.ToEntity();

        context.Products.Add(product);
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok(product.Id);
    }

    private static async Task<Results<BadRequest<string>, Ok>> UpdateProduct(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == id, ct);
        
        if (product is null)
        {
            return TypedResults.BadRequest<string>("Product not found");
        }

        request.UpdateEntity(product);
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }

    private static async Task<Results<BadRequest<string>, Ok>> DeleteProduct(
        [FromRoute] Guid id,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == id, ct);
        
        if (product is null)
        {
            return TypedResults.BadRequest<string>("Product not found");
        }
        
        context.Products.Remove(product);
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }
}