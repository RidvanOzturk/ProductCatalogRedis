using Microsoft.EntityFrameworkCore;
using ProductCatalogRedis.Data.Entities;

namespace ProductCatalogRedis.Data;

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> opts)
        : base(opts)
    { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(x =>
        {
            x.ToTable("Products");

            x.HasKey(e => e.Id);

            x.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(75);

            x.Property(e => e.Description)
                  .HasMaxLength(400);

            x.Property(e => e.Price)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            // trying index
            x.HasIndex(e => e.Name)
                  .HasDatabaseName("IX_Products_Name");
        });
    }
}
