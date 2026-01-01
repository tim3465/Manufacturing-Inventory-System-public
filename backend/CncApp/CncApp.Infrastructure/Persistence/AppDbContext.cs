
using CncApp.Domain.Entities;
//using CncApp.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    //FluentConfigurations.Configure(modelBuilder); // Old: used for FluentConfigurations static class

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    // Domain Users (distinct from Identity Users)
    public new DbSet<User> Users => Set<User>();
    public new DbSet<UserRole> UserRoles => Set<UserRole>();

    // Inventory
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<StockLot> StockLots => Set<StockLot>();
    public DbSet<StockLotAdjustment> StockLotAdjustments => Set<StockLotAdjustment>();

    // Production
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Shift> Shifts => Set<Shift>();


}
