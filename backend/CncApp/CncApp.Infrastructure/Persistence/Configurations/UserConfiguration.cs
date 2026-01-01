using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table name (preserve existing table name despite DbSet rename)
        builder.ToTable("Users");

        // Primary Key
        builder.HasKey(u => u.Id);

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

        // Navigation Properties
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        builder.HasMany(u => u.Shifts)
            .WithOne(s => s.Operator)
            .HasForeignKey(s => s.OperatorId);
    }
}

