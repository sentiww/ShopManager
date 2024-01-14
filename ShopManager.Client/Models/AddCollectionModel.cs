using ShopManager.Client.Requests;

namespace ShopManager.Client.Models;

public sealed class AddCollectionModel
{
    public string Name { get; set; }
}

public static class AddCollectionModelExtensions
{
    public static AddCollectionRequest ToRequest(this AddCollectionModel model)
    {
        return new AddCollectionRequest
        {
            Name = model.Name,
            ProductIds = Array.Empty<Guid>(),
            UserIds = Array.Empty<string>()
        };
    }
}