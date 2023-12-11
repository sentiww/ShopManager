using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShopManager.Persistence.Entity;

public sealed class ProductEntity : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int Quantity { get; set; }
    public ICollection<DiscountEntity> Discounts { get; set; } = null!;
    public ICollection<CollectionEntity> Collections { get; set; } = null!;
    public ICollection<ShopManagerUserEntity> Users { get; set; } = null!;
}

public sealed class ProductEntityConfiguration : BaseEntityConfiguration<ProductEntity>
{
    public new void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        base.Configure(builder);
        builder.Property(product => product.Price).IsRequired();
        builder.Property(product => product.Quantity).IsRequired();
        builder.HasMany(product => product.Discounts)
            .WithOne(discount => discount.Product)
            .HasForeignKey(discount => discount.Id);
        builder.HasMany(product => product.Collections)
            .WithMany(collection => collection.Products);
    }
}