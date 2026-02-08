using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> b)
        {
            b.ToTable("Orders");
            b.HasKey(x => x.Id);

            b.Property(x => x.Status).HasConversion<int>();

            b.OwnsOne(x => x.CustomerEmail, oe =>
            {
                oe.Property(p => p.Value).HasColumnName("CustomerEmail").HasMaxLength(256);
            });

            b.OwnsMany(x => x.Items, ib =>
            {
                ib.ToTable("OrderItems");
                ib.WithOwner().HasForeignKey("OrderId");

                ib.HasKey(x => x.Id);
                ib.Property(x => x.Id).ValueGeneratedNever();

                ib.Property<Guid>("OrderId");

                ib.Property(x => x.ProductId);

                ib.OwnsOne(x => x.UnitPrice, mp =>
                {
                    mp.Property(p => p.Amount).HasColumnName("UnitPriceAmount").HasPrecision(18, 2);
                    mp.Property(p => p.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(5);
                });

                ib.Property(x => x.Quantity);
            });
        }
    }
}
