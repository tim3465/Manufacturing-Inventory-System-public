using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class StockLotConfiguration : IEntityTypeConfiguration<StockLot>
{
    public void Configure(EntityTypeBuilder<StockLot> builder)
    {
        // Primary Key
        builder.HasKey(sl => sl.Id);

        // Properties
        builder.Property(sl => sl.LotNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sl => sl.MaterialId)
            .IsRequired();

        builder.Property(sl => sl.AmountOfBars)
            .IsRequired();

        builder.Property(sl => sl.Diameter)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(sl => sl.BarLength)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(sl => sl.Condition)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(sl => sl.CheckedInDateTime)
            .IsRequired();

        // Relationship
        builder.HasOne(sl => sl.Material)
            .WithMany(m => m.StockLots)
            .HasForeignKey(sl => sl.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on Foreign Key
        builder.HasIndex(sl => sl.MaterialId);

        // Navigation Properties
        builder.HasMany(sl => sl.StockLotAdjustments)
            .WithOne(sla => sla.StockLot)
            .HasForeignKey(sla => sla.StockLotId);
    }
}

