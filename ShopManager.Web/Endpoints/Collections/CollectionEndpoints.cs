using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Persistence;
using ShopManager.Web.Utilities;

namespace ShopManager.Web.Endpoints.Collections;

public static class CollectionEndpoints
{
    public static IEndpointRouteBuilder MapCollectionEndpoints(
        this IEndpointRouteBuilder builder,
        ApiVersionSet versionSet)
    {
        var group = builder.MapGroup("api/v{apiVersion:apiVersion}/collections");
        
        group.MapGet("", GetCollections)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapGet("{id}", GetCollection)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapPost("", CreateCollection)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapPut("{id}", UpdateCollection)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapDelete("{id}", DeleteCollection)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        
        return builder;
    }

    private static async Task<Ok<PagedCollection<CollectionDto>>> GetCollections(
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var collections = await context.Collections
            .Select(collection => new CollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
                Products = collection.Products
                    .Select(product => new CollectionDto.CollectionProductDto
                    {
                        Id = product.Id,
                        Name = product.Name
                    })
                    .ToList(),
                Users = collection.Users
                    .Select(user => new CollectionDto.CollectionUserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName
                    })
                    .ToList()
            })
            .ToPagedCollectionAsync(1, 10, ct);
        
        return TypedResults.Ok(collections);
    }

    private static async Task<Results<BadRequest<string>, Ok<CollectionDto>>> GetCollection(
        [FromRoute] Guid id,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var collection = await context.Collections
            .Select(collection => new CollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
                Products = collection.Products
                    .Select(product => new CollectionDto.CollectionProductDto
                    {
                        Id = product.Id,
                        Name = product.Name
                    })
                    .ToList(),
                Users = collection.Users
                    .Select(user => new CollectionDto.CollectionUserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(collection => collection.Id == id, ct);

        if (collection is null)
        {
            return TypedResults.BadRequest<string>("Collection not found");
        }

        return TypedResults.Ok(collection);
    }

    private static async Task<Results<BadRequest<string>, Ok<Guid>>> CreateCollection(
        [FromBody] AddCollectionRequest request,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var collection = request.ToEntity();

        var products = await context.Products
            .Where(product => request.ProductIds.Contains(product.Id))
            .ToListAsync(ct);
        
        if (products.Count != request.ProductIds.Length)
        {
            return TypedResults.BadRequest<string>("One or more products not found");
        }
        
        var users = await context.Users
            .Where(user => request.UserIds.Contains(user.Id))
            .ToListAsync(ct);
        
        if (users.Count != request.UserIds.Length)
        {
            return TypedResults.BadRequest<string>("One or more users not found");
        }
        
        collection.Products = products;
        collection.Users = users;
        
        context.Collections.Add(collection);

        await context.SaveChangesAsync(ct);

        return TypedResults.Ok(collection.Id);
    }

    private static async Task<Results<BadRequest<string>, Ok>> UpdateCollection(
        [FromRoute] Guid id,
        [FromBody] UpdateCollectionRequest request,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var collection = await context.Collections
            .Include(collection => collection.Products)
            .Include(collection => collection.Users)
            .FirstOrDefaultAsync(collection => collection.Id == id, ct);

        if (collection is null)
        {
            return TypedResults.BadRequest<string>("Collection not found");
        }
        
        request.UpdateEntity(collection);
        
        var products = await context.Products
            .Where(product => request.ProductIds.Contains(product.Id))
            .ToListAsync(ct);

        if (products.Count != request.ProductIds.Length)
        {
            return TypedResults.BadRequest<string>("One or more products not found");
        }
        
        var users = await context.Users
            .Where(user => request.UserIds.Contains(user.Id))
            .ToListAsync(ct);

        if (users.Count != request.UserIds.Length)
        {
            return TypedResults.BadRequest<string>("One or more users not found");
        }
        
        collection.Products = products;
        collection.Users = users;
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }

    private static async Task<Results<BadRequest<string>, Ok>> DeleteCollection(
        [FromRoute] Guid id,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var collection = await context.Collections
            .FirstOrDefaultAsync(collection => collection.Id == id, ct);

        if (collection is null)
        {
            return TypedResults.BadRequest<string>("Collection not found");
        }
        
        context.Collections.Remove(collection);
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }
}