using Microsoft.EntityFrameworkCore;
using GearShare.Domain.Entities;

namespace GearShare.Infrastructure.Data;

public class GearShareDbContext : DbContext
{
    public GearShareDbContext(DbContextOptions<GearShareDbContext> options) : base(options)
    {
    }

    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<PartCompatibility> PartCompatibilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Manufacturer entity
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Car entity
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId);
            entity.Property(e => e.Make).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(50);
            entity.Property(e => e.YearStart).IsRequired();
            entity.Property(e => e.YearEnd).IsRequired();
        });

        // Configure Part entity
        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(e => e.StockQuantity).IsRequired().HasDefaultValue(0);
            entity.HasIndex(e => e.Sku).IsUnique();
            
            // Foreign key relationship
            entity.HasOne(e => e.Manufacturer)
                  .WithMany(m => m.Parts)
                  .HasForeignKey(e => e.ManufacturerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure PartCompatibility entity (junction table)
        modelBuilder.Entity<PartCompatibility>(entity =>
        {
            entity.HasKey(e => new { e.PartId, e.CarId });
            
            // Foreign key relationships
            entity.HasOne(e => e.Part)
                  .WithMany(p => p.PartCompatibilities)
                  .HasForeignKey(e => e.PartId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Car)
                  .WithMany(c => c.PartCompatibilities)
                  .HasForeignKey(e => e.CarId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
