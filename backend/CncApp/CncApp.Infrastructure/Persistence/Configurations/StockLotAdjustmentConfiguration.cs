using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class StockLotAdjustmentConfiguration : IEntityTypeConfiguration<StockLotAdjustment>
{
    public void Configure(EntityTypeBuilder<StockLotAdjustment> builder)
    {
        // Primary Key
        builder.HasKey(sla => sla.StockLotAdjustmentId);

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

        // Owned Entity - AuditTrail
        builder.OwnsOne(sla => sla.AuditTrail, auditTrail =>
        {
            auditTrail.Property(a => a.CreatedAtDateTime)
                .IsRequired()
                .HasColumnName("AuditTrail_CreatedAtDateTime");

            auditTrail.Property(a => a.CreatedByUserId)
                .HasColumnName("AuditTrail_CreatedByUserId");

            auditTrail.Property(a => a.UpdatedAtDateTime)
                .HasColumnName("AuditTrail_UpdatedAtDateTime");

            auditTrail.Property(a => a.UpdatedByUserId)
                .HasColumnName("AuditTrail_UpdatedByUserId");

            auditTrail.Property(a => a.DisabledAtDateTime)
                .HasColumnName("AuditTrail_DisabledAtDateTime");

            auditTrail.Property(a => a.DisabledByUserId)
                .HasColumnName("AuditTrail_DisabledByUserId");
        });

        // Index on Foreign Key
        builder.HasIndex(sla => sla.StockLotId);
    }
}

