using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopManager.Common.Contracts;
using ShopManager.Common.Utilities;
using ShopManager.Persistence;
using ShopManager.Web.Endpoints.Collections;
using Testcontainers.PostgreSql;

namespace ShopManager.Web.Tests;

public class CollectionEndpoints
{
    private WebApplicationFactory<Program> _factory;
    private PostgreSqlContainer _npqsqlContainer;
    
    [OneTimeSetUp]
    public async Task Setup()
    {
        _npqsqlContainer = new PostgreSqlBuilder()
            .WithDatabase("shopmanager")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
        
        await _npqsqlContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Remove(services
                        .Single(descriptor => descriptor.ServiceType == typeof(DbContextOptions<ShopManagerContext>)));
                    services.Remove(services
                        .Single(descriptor => descriptor.ServiceType == typeof(ShopManagerContext)));

                    services.AddDbContext<ShopManagerContext>(options =>
                    {
                        options.UseNpgsql(_npqsqlContainer.GetConnectionString());
                    });
                });
            });
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _npqsqlContainer.DisposeAsync();
        await _factory.DisposeAsync();
    }

    [Test]
    public async Task Get_PagedCollections_ReturnsPagedCollections()
    {
        // Arrange
        using var client = _factory.CreateClient();
        
        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        
        var url = new Uri(client.BaseAddress, "/api/v1/collections");
        var query = new Dictionary<string, string>
        {
            {"page", "0"},
            {"pageSize", "10"}
        };
        var urlWithQueryParams = QueryHelpers.AddQueryString(url.ToString(), query);

        // Act
        var response = await client.GetAsync(urlWithQueryParams);
        
        var content = await response.Content.ReadFromJsonAsync<PagedCollection<CollectionDto>>();
        
        // Assert
        context.ChangeTracker.Clear();
        var collectionsCount = await context.Collections.CountAsync();
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Items.Count, Is.EqualTo(collectionsCount < 10 ? collectionsCount : 10));
        Assert.That(content.TotalCount, Is.EqualTo(collectionsCount));
        Assert.That(content.Page, Is.EqualTo(0));
        Assert.That(content.PageSize, Is.EqualTo(10));
    }

    [Test]
    public async Task Get_CollectionById_ReturnsCollection()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var collection = context.Collections.First();
        
        var url = new Uri(client.BaseAddress, $"/api/v1/collections/{collection.Id}");
        
        // Act
        var response = await client.GetAsync(url);
        
        var content = await response.Content.ReadFromJsonAsync<CollectionDto>();
        
        // Assert
        Assert.That(content, Is.Not.Null);
        Assert.That(content!.Id, Is.EqualTo(collection.Id));
    }
    
    [Test]
    public async Task Post_Collection_CreatesCollection()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var transaction = await context.Database.BeginTransactionAsync();
        
        var url = new Uri(client.BaseAddress, "/api/v1/collections");
        
        var request = new AddCollectionRequest
        {
            Name = "Test Collection",
            ProductIds = Array.Empty<Guid>(),
            UserIds = Array.Empty<string>()
        };
        
        // Act
        var response = await client.PostAsJsonAsync(url, request);
        
        var id = await response.Content.ReadFromJsonAsync<Guid>();
        
        // Assert
        context.ChangeTracker.Clear();
        var collection = await context.Collections
            .FirstOrDefaultAsync(collection => collection.Id == id);
        
        Assert.That(collection, Is.Not.Null);
        Assert.That(collection!.Name, Is.EqualTo(request.Name));
        
        await transaction.RollbackAsync();
    }
    
    [Test]
    public async Task Post_CollectionWithInvalidProductIds_ReturnsBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var transaction = await context.Database.BeginTransactionAsync();
        
        var url = new Uri(client.BaseAddress, "/api/v1/collections");
        
        var request = new AddCollectionRequest
        {
            Name = "Test Collection",
            ProductIds = new[] { Guid.NewGuid() },
            UserIds = Array.Empty<string>()
        };
        
        // Act
        var response = await client.PostAsJsonAsync(url, request);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        await transaction.RollbackAsync();
    }
    
    [Test]
    public async Task Post_CollectionWithInvalidUserIds_ReturnsBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var transaction = await context.Database.BeginTransactionAsync();
        
        var url = new Uri(client.BaseAddress, "/api/v1/collections");
        
        var request = new AddCollectionRequest
        {
            Name = "Test Collection",
            ProductIds = Array.Empty<Guid>(),
            UserIds = new[] { Guid.NewGuid().ToString() }
        };
        
        // Act
        var response = await client.PostAsJsonAsync(url, request);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        await transaction.RollbackAsync();
    }
    
    [Test]
    public async Task Put_Collection_UpdatesCollection()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var transaction = await context.Database.BeginTransactionAsync();
        
        var collection = context.Collections.First();
        
        var url = new Uri(client.BaseAddress, $"/api/v1/collections/{collection.Id}");
        
        var request = new UpdateCollectionRequest
        {
            Name = "Test Collection",
            ProductIds = Array.Empty<Guid>(),
            UserIds = Array.Empty<string>()
        };
        
        // Act
        var response = await client.PutAsJsonAsync(url, request);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        context.ChangeTracker.Clear();
        var updatedCollection = await context.Collections
            .FirstOrDefaultAsync(dbCollection => dbCollection.Id == collection.Id);
        
        Assert.That(updatedCollection, Is.Not.Null);
        Assert.That(updatedCollection!.Name, Is.EqualTo(request.Name));
        
        await transaction.RollbackAsync();
    }
    
    [Test]
    public async Task Put_CollectionWithInvalidProductIds_ReturnsBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var transaction = await context.Database.BeginTransactionAsync();
        
        var collection = context.Collections.First();
        
        var url = new Uri(client.BaseAddress, $"/api/v1/collections/{collection.Id}");
        
        var request = new UpdateCollectionRequest
        {
            Name = "Test Collection",
            ProductIds = new[] { Guid.NewGuid() },
            UserIds = Array.Empty<string>()
        };
        
        // Act
        var response = await client.PutAsJsonAsync(url, request);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        await transaction.RollbackAsync();
    }
    
    [Test]
    public async Task Put_CollectionWithInvalidUserIds_ReturnsBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var transaction = await context.Database.BeginTransactionAsync();
        
        var collection = context.Collections.First();
        
        var url = new Uri(client.BaseAddress, $"/api/v1/collections/{collection.Id}");
        
        var request = new UpdateCollectionRequest
        {
            Name = "Test Collection",
            ProductIds = Array.Empty<Guid>(),
            UserIds = new[] { Guid.NewGuid().ToString() }
        };
        
        // Act
        var response = await client.PutAsJsonAsync(url, request);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        await transaction.RollbackAsync();
    }
    
    [Test]
    public async Task Delete_Collection_DeletesCollection()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var servicesScope = _factory.Services.CreateScope();
        var context = servicesScope.ServiceProvider.GetRequiredService<ShopManagerContext>();
        var transaction = await context.Database.BeginTransactionAsync();
        
        var collection = context.Collections.First();
        
        var url = new Uri(client.BaseAddress, $"/api/v1/collections/{collection.Id}");
        
        // Act
        var response = await client.DeleteAsync(url);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        context.ChangeTracker.Clear();
        var deletedCollection = await context.Collections
            .FirstOrDefaultAsync(dbCollection => dbCollection.Id == collection.Id);
        
        Assert.That(deletedCollection, Is.Null);
        
        await transaction.RollbackAsync();
    }
}