using ShopManager.Common.Requests;

namespace ShopManager.Client.Models;

public sealed class AddCollectionModel
{
    public string Name { get; set; }
}

public static class AddCollectionModelExtensions
{
    public static CreateCollectionRequest ToRequest(this AddCollectionModel model)
    {
        return new CreateCollectionRequest
        {
            Name = model.Name,
            ProductIds = Array.Empty<Guid>(),
            UserIds = Array.Empty<string>()
        };
    }
}