using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShopManager.Persistence.Entity;

public sealed class CollectionEntity : BaseEntity
{
    public required string Name { get; set; } = null!;
    public ICollection<ProductEntity> Products { get; set; } = null!;
    public ICollection<ShopManagerUserEntity> Users { get; set; } = null!;
}

public sealed class CollectionEntityConfiguration : BaseEntityConfiguration<CollectionEntity>
{
    public new void Configure(EntityTypeBuilder<CollectionEntity> builder)
    {
        base.Configure(builder);
        builder.Property(collection => collection.Name).IsRequired();
        builder.HasMany(collection => collection.Users)
            .WithMany(product => product.Collections);
    }
}