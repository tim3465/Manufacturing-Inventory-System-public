using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class ShiftIssueLogConfiguration : IEntityTypeConfiguration<ShiftIssueLog>
{
    public void Configure(EntityTypeBuilder<ShiftIssueLog> builder)
    {
        // Primary Key
        builder.HasKey(sil => sil.Id);

        // Properties
        builder.Property(sil => sil.ShiftId)
            .IsRequired();

        builder.Property(sil => sil.IssueType)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(sil => sil.ScrapQuantity)
            .IsRequired();

        builder.Property(sil => sil.Downtime);

        builder.Property(sil => sil.Description)
            .IsRequired()
            .HasMaxLength(2000);

        // Relationship
        builder.HasOne(sil => sil.Shift)
            .WithMany(s => s.ShiftIssueLogs)
            .HasForeignKey(sil => sil.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on Foreign Key
        builder.HasIndex(sil => sil.ShiftId);
    }
}
