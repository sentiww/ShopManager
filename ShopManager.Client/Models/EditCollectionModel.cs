using ShopManager.Common.Requests;

namespace ShopManager.Client.Models;

public sealed class EditCollectionModel
{
    public string Name { get; set; }
    public Guid[] ProductIds { get; set; }
    public string[] UserIds { get; set; }
}

public static class EditCollectionModelExtensions
{
    public static UpdateCollectionRequest ToRequest(this EditCollectionModel model)
    {
        return new UpdateCollectionRequest
        {
            Name = model.Name,
            ProductIds = model.ProductIds,
            UserIds = model.UserIds
        };
    }
}