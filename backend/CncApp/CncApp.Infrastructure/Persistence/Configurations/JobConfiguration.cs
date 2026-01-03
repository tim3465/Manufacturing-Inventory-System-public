using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        // Primary Key
        builder.HasKey(j => j.Id);

        // Properties
        builder.Property(j => j.OrderId)
            .IsRequired();

        builder.Property(j => j.StockLotId)
            .IsRequired();

        builder.Property(j => j.MachineId)
            .IsRequired();

        builder.Property(j => j.PartAmountPlanned)
            .IsRequired();

        builder.Property(j => j.BarAmountPlanned)
            .IsRequired();

        builder.Property(j => j.BarCycleTime)
            .IsRequired();

        builder.Property(j => j.BarsInJob)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(j => j.EstimatedPartsPerBar);

        // Relationships
        builder.HasOne(j => j.Order)
            .WithMany(o => o.Jobs)
            .HasForeignKey(j => j.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(j => j.StockLot)
            .WithMany()
            .HasForeignKey(j => j.StockLotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(j => j.Machine)
            .WithMany(m => m.Jobs)
            .HasForeignKey(j => j.MachineId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes on Foreign Keys
        builder.HasIndex(j => j.OrderId);
        builder.HasIndex(j => j.StockLotId);
        builder.HasIndex(j => j.MachineId);

        // Navigation Properties
        builder.HasMany(j => j.Shifts)
            .WithOne(s => s.Job)
            .HasForeignKey(s => s.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

