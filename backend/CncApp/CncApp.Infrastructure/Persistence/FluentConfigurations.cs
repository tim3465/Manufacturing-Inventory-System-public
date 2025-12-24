using CncApp.Domain.Common;
using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace CncApp.Infrastructure.Persistence.Configurations;

public static class FluentConfigurations
{

    public static void Configure(ModelBuilder modelBuilder)
    {
        ConfigureUserRole(modelBuilder);
        ConfigureStockLot(modelBuilder);
        ConfigureAuditTrail(modelBuilder);
        ConfigureShift(modelBuilder);

    }

    // AuditTrail
    private static void ConfigureAuditTrail(ModelBuilder modelBuilder)
    {
        modelBuilder.Owned<AuditTrail>();
    }

    // UserRole
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

    //StockLot
    private static void ConfigureStockLot(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockLot>()
            .Property(x => x.Diameter)
            .HasPrecision(18, 4);

        modelBuilder.Entity<StockLot>()
            .Property(x => x.BarLength)
            .HasPrecision(18, 4);
    }

    // Shift
    private static void ConfigureShift(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shift>()
            .HasOne(s => s.Operator)
            .WithMany(u => u.Shifts)
            .HasForeignKey(s => s.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }


}
