using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopManager.Common.Contracts;
using ShopManager.Common.Utilities;
using ShopManager.Persistence.Entity;

namespace ShopManager.Web.Endpoints.Users;


public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder builder,
        ApiVersionSet versionSet)
    {
        var group = builder.MapGroup("api/v{apiVersion:apiVersion}/users");

        group.MapPost(string.Empty, CreateUser)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);

        group.MapPost("login", Login)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        
        group.MapGet(string.Empty, GetUsers)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);
        
        group.MapGet("{id}", GetUser)
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1);

        return builder;
    }

    private static async Task<Ok<IdentityResult>> CreateUser(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromBody] CreateUserRequest request)
    {
        var user = new ShopManagerUserEntity
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<JwtResponse>, UnauthorizedHttpResult>> Login(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(userManager.NormalizeEmail(request.Email));
        if (user is null)
            return TypedResults.Unauthorized();

        var result = await userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
            return TypedResults.Unauthorized();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                "super secret key asdsdhdfhdfhdfhdfhdfhsdfhsdfhaserasdfhdzxftggzsdfghdfgxhszrfsZgh"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            "http://localhost:8000",
            "http://localhost:3000",
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return TypedResults.Ok(new JwtResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
    
    private static async Task<Ok<PagedCollection<UserDto>>> GetUsers(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromQuery] int page,
        [FromQuery] int pageSize)
    {
        return TypedResults.Ok(await userManager.Users
            .OrderBy(user => user.UserName)
            .Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            })
            .ToPagedCollectionAsync(page, pageSize));
    }
    
    private static async Task<Results<Ok<UserDto>, BadRequest<string>>> GetUser(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromRoute] string id)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user is null)
        {
            return TypedResults.BadRequest("User not found");
        }

        return TypedResults.Ok(new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        });
    }
}