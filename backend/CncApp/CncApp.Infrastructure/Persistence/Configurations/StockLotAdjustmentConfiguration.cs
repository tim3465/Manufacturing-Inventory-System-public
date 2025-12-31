using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class StockLotAdjustmentConfiguration : IEntityTypeConfiguration<StockLotAdjustment>
{
    public void Configure(EntityTypeBuilder<StockLotAdjustment> builder)
    {
        // Primary Key
        builder.HasKey(sla => sla.Id);

        // Properties
        builder.Property(sla => sla.StockLotId)
            .IsRequired();

        builder.Property(sla => sla.JobId);

        builder.Property(sla => sla.DeltaBars)
            .IsRequired();

        builder.Property(sla => sla.Reason)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(sla => sla.Notes)
            .HasMaxLength(2000);

        // Relationship
        builder.HasOne(sla => sla.StockLot)
            .WithMany(sl => sl.StockLotAdjustments)
            .HasForeignKey(sla => sla.StockLotId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on Foreign Key
        builder.HasIndex(sla => sla.StockLotId);
    }
}

