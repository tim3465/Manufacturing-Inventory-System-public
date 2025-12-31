using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        // Primary Key
        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.JobId)
            .IsRequired();

        builder.Property(s => s.OperatorId)
            .IsRequired();

        builder.Property(s => s.PartsMade);

        builder.Property(s => s.Scrap);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.StopTime);

        builder.Property(s => s.Downtime);

        // Relationships
        builder.HasOne(s => s.Job)
            .WithMany(j => j.Shifts)
            .HasForeignKey(s => s.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Operator)
            .WithMany(u => u.Shifts)
            .HasForeignKey(s => s.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes on Foreign Keys
        builder.HasIndex(s => s.JobId);
        builder.HasIndex(s => s.OperatorId);
    }
}

