using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Common.Contracts;
using ShopManager.Common.Utilities;
using ShopManager.Persistence;
using ShopManager.Web.Common;

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
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? sortLabel,
        [FromQuery] int? sortDirection,
        [FromQuery] string? searchString,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var query = context.Products
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            });
            
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            var lowerSearchString = searchString.ToLower();
            query = query.Where(product => product.Id.ToString().Contains(lowerSearchString) ||
                                           product.Name.ToLower().Contains(lowerSearchString) ||
                                           product.Description.ToLower().Contains(lowerSearchString) ||
                                           product.Price.ToString().Contains(lowerSearchString) ||
                                           product.Quantity.ToString().Contains(lowerSearchString));
        }

        var orderDirection = SortDirection.Ascending;
        if (sortDirection.HasValue)
        {
            orderDirection = (SortDirection)sortDirection.Value;
        }
        
        if (!string.IsNullOrWhiteSpace(sortLabel))
        {
            switch (orderDirection)
            {
                case SortDirection.None:
                    break;
                case SortDirection.Ascending:
                    query = sortLabel switch
                    {
                        "id" => query.OrderBy(product => product.Id),
                        "name" => query.OrderBy(product => product.Name),
                        "description" => query.OrderBy(product => product.Description),
                        "price" => query.OrderBy(product => product.Price),
                        "quantity" => query.OrderBy(product => product.Quantity),
                        _ => query
                    };
                    break;
                case SortDirection.Descending:
                    query = sortLabel switch
                    {
                        "id" => query.OrderByDescending(product => product.Id),
                        "name" => query.OrderByDescending(product => product.Name),
                        "description" => query.OrderByDescending(product => product.Description),
                        "price" => query.OrderByDescending(product => product.Price),
                        "quantity" => query.OrderByDescending(product => product.Quantity),
                        _ => query
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortDirection), sortDirection, null);
            }
        }
    
        var products = await query
            .ToPagedCollectionAsync(page, pageSize, ct);
        
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