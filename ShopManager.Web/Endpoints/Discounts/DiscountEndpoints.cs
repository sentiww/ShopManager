using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopManager.Persistence;
using ShopManager.Web.Utilities;

namespace ShopManager.Web.Endpoints.Discounts;

public static class DiscountEndpoints
{
    public static IEndpointRouteBuilder MapDiscountEndpoints(
        this IEndpointRouteBuilder builder,
        ApiVersionSet versionSet)
    {
        var group = builder.MapGroup("api/v{apiVersion:apiVersion}/discounts");

        group.MapGet("", GetDiscounts)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapGet("{id}", GetDiscount)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapPost("", CreateDiscount)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapPut("{id}", UpdateDiscount)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        group.MapDelete("{id}", DeleteDiscount)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        
        return group;
    }

    private static async Task<Ok<PagedCollection<DiscountDto>>> GetDiscounts(
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var discounts = await context.Discounts
            .Select(discount => new DiscountDto
            {
                Id = discount.Id,
                Percentage = discount.Percentage,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                ProductId = discount.ProductId
            })
            .OrderBy(discount => discount.Id)
            .ToPagedCollectionAsync(1, 10, ct);
        
        return TypedResults.Ok(discounts);
    }

    private static async Task<Results<BadRequest<string>, Ok<DiscountDto>>> GetDiscount(
        [FromRoute] Guid id,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var discount = await context.Discounts
            .Select(discount => new DiscountDto
            {
                Id = discount.Id,
                Percentage = discount.Percentage,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                ProductId = discount.ProductId
            })
            .FirstOrDefaultAsync(discount => discount.Id == id, ct);

        if (discount is null)
        {
            return TypedResults.BadRequest("Discount not found");
        }
        
        return TypedResults.Ok(discount);
    }

    private static async Task<Results<BadRequest<string>, Ok<Guid>>> CreateDiscount(
        [FromBody] CreateDiscountRequest request,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == request.ProductId, ct);
        
        if (product is null)
        {
            return TypedResults.BadRequest("Product not found");
        }
        
        var discount = request.ToEntity();
        
        context.Discounts.Add(discount);
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok(discount.Id);
    }

    private static async Task<Results<BadRequest<string>, Ok>> UpdateDiscount(
        [FromRoute] Guid id,
        [FromBody] UpdateDiscountRequest request,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var discount = await context.Discounts
            .FirstOrDefaultAsync(discount => discount.Id == id, ct);

        if (discount is null)
        {
            return TypedResults.BadRequest("Discount not found");
        }
        
        request.UpdateEntity(discount);
        
        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == discount.ProductId, ct);
        
        if (product is null)
        {
            return TypedResults.BadRequest("Product not found");
        }
        
        discount.Product = product;
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }

    private static async Task<Results<BadRequest<string>, Ok>> DeleteDiscount(
        [FromRoute] Guid id,
        [FromServices] ShopManagerContext context,
        CancellationToken ct)
    {
        var discount = await context.Discounts
            .FirstOrDefaultAsync(discount => discount.Id == id, ct);

        if (discount is null)
        {
            return TypedResults.BadRequest("Discount not found");
        }
        
        context.Discounts.Remove(discount);
        
        await context.SaveChangesAsync(ct);
        
        return TypedResults.Ok();
    }
}