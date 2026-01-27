using Microsoft.EntityFrameworkCore;
using OrderManagement.Inventory.Core.Entities;

namespace OrderManagement.Inventory.Infrastructure.Persistence;

public class InventoryDbContext : DbContext
{

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<StockItem> StockItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<StockItem>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Sku)
                .IsRequired()
                .HasMaxLength(50);

            e.HasIndex(x => x.Sku)
                .IsUnique();

            e.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);
        });

        base.OnModelCreating(builder);
    }
}