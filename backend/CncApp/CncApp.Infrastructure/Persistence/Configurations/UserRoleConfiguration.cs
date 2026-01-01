using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Table name (preserve existing table name despite DbSet rename)
        builder.ToTable("UserRoles");

        // Primary Key
        builder.HasKey(ur => ur.Id);

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

        // Unique Index
        builder.HasIndex(ur => new { ur.UserId, ur.RoleType })
            .IsUnique()
            .HasFilter("[InactivatedDateTime] IS NULL");
    }
}

