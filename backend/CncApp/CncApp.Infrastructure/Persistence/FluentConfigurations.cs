using Microsoft.EntityFrameworkCore;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;


namespace CncApp.Infrastructure.Persistence.Configurations;

public static class FluentConfigurations
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        ConfigureUserRole(modelBuilder);
        ConfigureStockLot(modelBuilder);
    }

    private static void ConfigureUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserRole>()
            .HasIndex(x => new { x.UserId, x.RoleType })
            .IsUnique()
            .HasFilter("[AuditTrail_DisabledAtDateTime] IS NULL");
    }

    private static void ConfigureStockLot(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockLot>()
            .Property(x => x.Diameter)
            .HasPrecision(18, 4);

        modelBuilder.Entity<StockLot>()
            .Property(x => x.BarLength)
            .HasPrecision(18, 4);
    }
}
