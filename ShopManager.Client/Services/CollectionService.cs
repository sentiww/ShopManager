using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using ShopManager.Client.Common;
using ShopManager.Client.Dtos;
using ShopManager.Client.Requests;

namespace ShopManager.Client.Services;

public interface ICollectionService
{
    public Task<PaginatedResonse<CollectionDto>> GetCollectionsAsync(
        int page, 
        int pageSize, 
        string? sortLabel, 
        SortDirection? sortDirection, 
        string? searchString);
    public Task<CollectionDetailsDto> GetCollectionAsync(Guid id);
    public Task AddCollectionAsync(AddCollectionRequest request);
    public Task EditCollectionAsync(Guid id, EditCollectionRequest request);
    public Task RemoveCollectionAsync(Guid id);
}

internal sealed class CollectionService : ICollectionService
{
    private readonly HttpClient _httpClient;

    public CollectionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<PaginatedResonse<CollectionDto>> GetCollectionsAsync(
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

        var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("http://localhost:8000/api/v1/collections", query));
        
        var collections = await response.Content.ReadFromJsonAsync<PaginatedResonse<CollectionDto>>();
        
        if (collections is null)
        {
            throw new Exception("Failed to get collections");
        }
        
        return collections;
    }

    public async Task<CollectionDetailsDto> GetCollectionAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"http://localhost:8000/api/v1/collections/{id}");
        
        var collection = await response.Content.ReadFromJsonAsync<CollectionDetailsDto>();
        
        if (collection is null)
        {
            throw new Exception("Failed to get collection");
        }
        
        return collection;
    }

    public async Task AddCollectionAsync(AddCollectionRequest request)
    {
        await _httpClient.PostAsJsonAsync("http://localhost:8000/api/v1/collections", request);
    }

    public async Task EditCollectionAsync(Guid id, EditCollectionRequest request)
    {
        await _httpClient.PutAsJsonAsync($"http://localhost:8000/api/v1/collections/{id}", request);
    }

    public async Task RemoveCollectionAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"http://localhost:8000/api/v1/collections/{id}");
    }
}