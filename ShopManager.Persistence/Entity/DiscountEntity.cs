using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShopManager.Persistence.Entity;

public sealed class DiscountEntity : BaseEntity
{
    public required Guid ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public required decimal Percentage { get; set; }
    public required DateTimeOffset StartDate { get; set; }
    public required DateTimeOffset EndDate { get; set; }
}

public sealed class DiscountEntityConfiguration : BaseEntityConfiguration<DiscountEntity>
{
    public new void Configure(EntityTypeBuilder<DiscountEntity> builder)
    {
        base.Configure(builder);
        builder.Property(discount => discount.Percentage).IsRequired();
        builder.Property(discount => discount.StartDate).IsRequired();
        builder.Property(discount => discount.EndDate).IsRequired();
    }
}