using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Primary Key
        builder.HasKey(ur => ur.UserRoleId);

        // Properties
        builder.Property(ur => ur.UserId)
            .IsRequired();

        builder.Property(ur => ur.RoleType)
            .IsRequired()
            .HasConversion<byte>();

        // Relationship
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Owned Entity - AuditTrail
        builder.OwnsOne(ur => ur.AuditTrail, auditTrail =>
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

        // Unique Index
        builder.HasIndex(ur => new { ur.UserId, ur.RoleType })
            .IsUnique()
            .HasFilter("[AuditTrail_DisabledAtDateTime] IS NULL");
    }
}

