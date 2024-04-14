using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using ShopManager.Client.Common;
using ShopManager.Client.Dtos;
using ShopManager.Client.Requests;
using ShopManager.Common.Contracts;
using ShopManager.Common.Utilities;

namespace ShopManager.Client.Services;

public interface IProductService
{
    public Task<PagedCollection<ProductDto>> GetProductsAsync(
        int page, 
        int pageSize, 
        string? sortLabel, 
        SortDirection? sortDirection, 
        string? searchString);
    public Task<ProductDto> GetProductAsync(Guid id);
    public Task AddProductAsync(AddProductRequest request);
    public Task RemoveProductAsync(Guid id);
    public Task EditProductAsync(Guid id, EditProductRequest request);
}

internal sealed class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<PagedCollection<ProductDto>> GetProductsAsync(
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

        var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("http://localhost:8000/api/v1/products", query));
        
        var products = await response.Content.ReadFromJsonAsync<PagedCollection<ProductDto>>();

        if (products is null)
        {
            throw new Exception("Failed to get products");
        }
        
        return products;
    }

    public async Task<ProductDto> GetProductAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"http://localhost:8000/api/v1/products/{id}");
        
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        
        if (product is null)
        {
            throw new Exception("Failed to get product");
        }
        
        return product;
    }
    
    public async Task AddProductAsync(AddProductRequest request)
    {
        await _httpClient.PostAsJsonAsync("http://localhost:8000/api/v1/products", request);
    }

    public async Task RemoveProductAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"http://localhost:8000/api/v1/products/{id}");
    }

    public async Task EditProductAsync(Guid id, EditProductRequest request)
    {
        await _httpClient.PutAsJsonAsync($"http://localhost:8000/api/v1/products/{id}", request);
    }
}