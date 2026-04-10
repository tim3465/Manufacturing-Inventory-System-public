using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CncApp.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.CompanyName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(200);

        // Relationship defined on OrderConfiguration side
    }
}
