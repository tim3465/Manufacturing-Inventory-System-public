using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        // Primary Key
        builder.HasKey(m => m.MachineId);

        // Properties
        builder.Property(m => m.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.ModelNumber)
            .IsRequired()
            .HasMaxLength(100);

        // Navigation Properties
        builder.HasMany(m => m.Jobs)
            .WithOne(j => j.Machine)
            .HasForeignKey(j => j.MachineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

