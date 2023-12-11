using Microsoft.AspNetCore.Identity;

namespace ShopManager.Persistence.Entity;

public class ShopManagerUserEntity : IdentityUser
{
    public ICollection<CollectionEntity> Collections { get; set; } = null!;
}