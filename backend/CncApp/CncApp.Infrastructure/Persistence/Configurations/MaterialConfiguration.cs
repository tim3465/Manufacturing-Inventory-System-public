using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        // Primary Key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.HeatNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.MaterialName)
            .IsRequired()
            .HasMaxLength(100);

        // Navigation Properties
        builder.HasMany(m => m.StockLots)
            .WithOne(sl => sl.Material)
            .HasForeignKey(sl => sl.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

