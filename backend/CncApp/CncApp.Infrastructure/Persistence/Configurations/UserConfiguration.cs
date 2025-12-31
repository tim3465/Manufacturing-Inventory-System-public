using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary Key
        builder.HasKey(u => u.UserId);

        // Properties
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.FirstName)
            .HasMaxLength(200);

        builder.Property(u => u.LastName)
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .HasMaxLength(320);

        // Owned Entity - AuditTrail
        builder.OwnsOne(u => u.AuditTrail, auditTrail =>
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

        // Navigation Properties
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        builder.HasMany(u => u.Shifts)
            .WithOne(s => s.Operator)
            .HasForeignKey(s => s.OperatorId);
    }
}

