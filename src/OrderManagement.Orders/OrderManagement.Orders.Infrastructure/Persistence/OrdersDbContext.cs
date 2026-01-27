using Microsoft.EntityFrameworkCore;
using OrderManagement.Orders.Core.Entities;

namespace OrderManagement.Orders.Infrastructure.Persistence;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Status)
                .IsRequired();

            e.Property(x => x.ValidationReason)
                .HasMaxLength(1000);

            e.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderItem>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Sku)
                .IsRequired()
                .HasMaxLength(50);

            e.Property(x => x.Quantity)
                .IsRequired();
        });

        base.OnModelCreating(builder);
    }
}
