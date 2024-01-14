using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopManager.Persistence.Entity;
using ShopManager.Web.Utilities;

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

    private static async Task<IdentityResult> CreateUser(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromBody] CreateUserRequest request)
    {
        var user = new ShopManagerUserEntity
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        return result;
    }

    private static async Task<JwtResponse> Login(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(userManager.NormalizeEmail(request.Email));
        if (user is null)
            throw new UnauthorizedAccessException();

        var result = await userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
            throw new UnauthorizedAccessException();

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

        return new JwtResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }
    
    private static Task<PagedCollection<UserDto>> GetUsers(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromQuery] int page,
        [FromQuery] int pageSize)
    {
        return userManager.Users
            .OrderBy(user => user.UserName)
            .Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            })
            .ToPagedCollectionAsync(page, pageSize);
    }
    
    private static async Task<UserDto> GetUser(
        [FromServices] UserManager<ShopManagerUserEntity> userManager,
        [FromRoute] string id)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user is null)
        {
            throw new Exception("User not found");
        }

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }
}