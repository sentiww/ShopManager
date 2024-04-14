using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using ShopManager.Client.Common;
using ShopManager.Client.Dtos;
using ShopManager.Client.Providers;
using ShopManager.Client.Requests;
using ShopManager.Common.Contracts;
using ShopManager.Common.Utilities;

namespace ShopManager.Client.Services;

public interface IUserService
{
    public Task RegisterAsync(RegisterRequest request);
    public Task<JwtDto> LoginAsync(LoginRequest request);
    public Task LogoutAsync();
    public Task<UserDto> GetUserAsync(string id);
    public Task<PagedCollection<UserDto>> GetUsersAsync(int page, int pageSize, string? searchString);
}

internal sealed class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;
    
    public UserService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider, 
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }
    
    public async Task RegisterAsync(RegisterRequest request)
    {
        await _httpClient.PostAsJsonAsync("http://localhost:8000/api/v1/users", request);
    }
    
    public async Task<JwtDto> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:8000/api/v1/users/login", request);
        
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();

        if (jwt is null)
        {
            throw new Exception("Invalid response");
        }
        
        await _localStorage.SetItemAsync("authToken", jwt.Token);
        
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwt.Token);
        var tokenS = jsonToken as JwtSecurityToken;
        var email = tokenS?.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
        
        ((ClientAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(email);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", jwt.Token);
        
        return jwt;
    }
    
    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((ClientAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<UserDto> GetUserAsync(string id)
    {
        var response = await _httpClient.GetAsync($"http://localhost:8000/api/v1/users/{id}");
        
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        
        if (user is null)
        {
            throw new Exception("User not found");
        }
        
        return user;
    }

    public async Task<PagedCollection<UserDto>> GetUsersAsync(int page, int pageSize, string? searchString)
    {
        var query = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        };
        
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query.Add("searchString", searchString);
        }
        
        var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("http://localhost:8000/api/v1/users", query)); 
        
        var users = await response.Content.ReadFromJsonAsync<PagedCollection<UserDto>>();
        
        if (users is null)
        {
            throw new Exception("Users not found");
        }
        
        return users;
    }
}