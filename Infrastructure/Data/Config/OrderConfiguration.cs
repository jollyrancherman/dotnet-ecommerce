using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // We want to store the order id as a string in the database
            // so we need to tell EF how to convert it
            builder.OwnsOne(
                o => o.ShipToAddress,
                a => 
                {
                    a.WithOwner();
                });
            
            builder.Navigation(a => a.ShipToAddress).IsRequired();

            // OrderStatus is an enum, but we want to store it as a string in the database
            // so we need to tell EF how to convert it
            builder.Property(s=>s.Status)
                .HasConversion(
                    o => o.ToString(),
                    o => (OrderStatus) Enum.Parse(typeof(OrderStatus), o)
                );

            // OrderItems will be deleted when the order is deleted (cascade)
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}