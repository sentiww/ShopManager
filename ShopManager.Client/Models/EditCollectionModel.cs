using ShopManager.Client.Requests;

namespace ShopManager.Client.Models;

public sealed class EditCollectionModel
{
    public string Name { get; set; }
    public Guid[] ProductIds { get; set; }
    public string[] UserIds { get; set; }
}

public static class EditCollectionModelExtensions
{
    public static EditCollectionRequest ToRequest(this EditCollectionModel model)
    {
        return new EditCollectionRequest
        {
            Name = model.Name,
            ProductIds = model.ProductIds,
            UserIds = model.UserIds
        };
    }
}