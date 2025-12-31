using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Primary Key
        builder.HasKey(o => o.OrderId);

        // Properties
        builder.Property(o => o.PartId)
            .IsRequired();

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.PartAmountRequested)
            .IsRequired();

        builder.Property(o => o.PartsPerBar);

        // Relationship
        builder.HasOne(o => o.Part)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PartId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on Foreign Key
        builder.HasIndex(o => o.PartId);

        // Navigation Properties
        builder.HasMany(o => o.Jobs)
            .WithOne(j => j.Order)
            .HasForeignKey(j => j.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

