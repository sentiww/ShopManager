using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using ShopManager.Client.Common;
using ShopManager.Client.Dtos;
using ShopManager.Client.Requests;

namespace ShopManager.Client.Services;

public interface IDiscountService
{
    public Task<PaginatedResonse<DiscountDto>> GetDiscountsAsync(
        int page, 
        int pageSize, 
        string? sortLabel, 
        SortDirection? sortDirection,
        string? searchString);
    public Task<DiscountDto> GetDiscountAsync(Guid id);
    public Task AddDiscountAsync(AddDiscountRequest request);
    public Task EditDiscountAsync(Guid id, EditDiscountRequest request);
    public Task RemoveDiscountAsync(Guid id);
}

internal sealed class DiscountService : IDiscountService
{
    private readonly HttpClient _httpClient;
    
    public DiscountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<PaginatedResonse<DiscountDto>> GetDiscountsAsync(
        int page, 
        int pageSize, 
        string? sortLabel, 
        SortDirection? sortDirection,
        string? searchString)
    {
        var query = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        };
        
        if (!string.IsNullOrWhiteSpace(sortLabel))
        {
            query["sortLabel"] = sortLabel;
        }
        
        if (sortDirection.HasValue)
        {
            query["sortDirection"] = ((int)sortDirection.Value).ToString();
        }
        
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query["searchString"] = searchString;
        }
        
        var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("http://localhost:8000/api/v1/discounts", query));
        
        var discounts = await response.Content.ReadFromJsonAsync<PaginatedResonse<DiscountDto>>();
        
        if (discounts is null)
        {
            throw new Exception("Failed to get discounts");
        }
        
        return discounts;
    }

    public async Task<DiscountDto> GetDiscountAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"http://localhost:8000/api/v1/discounts/{id}");
        
        var discount = await response.Content.ReadFromJsonAsync<DiscountDto>();
        
        if (discount is null)
        {
            throw new Exception("Failed to get discount");
        }
        
        return discount;
    }

    public async Task AddDiscountAsync(AddDiscountRequest request)
    {
        await _httpClient.PostAsJsonAsync("http://localhost:8000/api/v1/discounts", request);
    }

    public async Task EditDiscountAsync(Guid id, EditDiscountRequest request)
    {
        await _httpClient.PutAsJsonAsync($"http://localhost:8000/api/v1/discounts/{id}", request);
    }

    public async Task RemoveDiscountAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"http://localhost:8000/api/v1/discounts/{id}");
    }
}