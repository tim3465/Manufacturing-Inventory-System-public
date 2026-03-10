using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.PartName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PartNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.PartNumber)
            .IsUnique();

        builder.Property(p => p.ApproxPartCycleTime)
            .IsRequired();

        builder.Property(p => p.CheckPerPart)
            .IsRequired();

        // Navigation Properties
        builder.HasMany(p => p.Orders)
            .WithOne(o => o.Part)
            .HasForeignKey(o => o.PartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

