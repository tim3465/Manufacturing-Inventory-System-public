using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Primary Key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.PartId)
            .IsRequired();

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.PartAmountRequested)
            .IsRequired();

        builder.Property(o => o.PartsPerBar);

        // Relationships
        builder.HasOne(o => o.Part)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes on Foreign Keys
        builder.HasIndex(o => o.PartId);
        builder.HasIndex(o => o.CustomerId);

        // Navigation Properties
        builder.HasMany(o => o.Jobs)
            .WithOne(j => j.Order)
            .HasForeignKey(j => j.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

