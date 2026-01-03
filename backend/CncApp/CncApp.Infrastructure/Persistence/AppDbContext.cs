
using CncApp.Application.Interfaces;
using CncApp.Domain.Common;
using CncApp.Domain.Entities;
//using CncApp.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
{
    private int? _cachedCurrentDomainUserId;
    private bool _hasResolvedCurrentDomainUserId;

    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    //FluentConfigurations.Configure(modelBuilder); // Old: used for FluentConfigurations static class

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    // Domain Users (distinct from Identity Users)
    public DbSet<User> DomainUsers => Set<User>();

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

    /// <summary>
    /// Overrides SaveChangesAsync to automatically populate audit fields with DomainUserId.
    /// Translates IdentityUserId (from JWT) to DomainUserId (for audit fields).
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var currentDomainUserId = await GetCurrentDomainUserIdAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        // Process all entities that are being added or modified
        foreach (var entry in ChangeTracker.Entries<AuditableEntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Set creation audit fields
                    entry.Entity.CreatedDateTime = now;
                    if (currentDomainUserId.HasValue)
                    {
                        entry.Entity.CreatedByUserId = currentDomainUserId.Value;
                    }
                    break;

                case EntityState.Modified:
                    // Set update audit fields
                    entry.Entity.UpdatedDateTime = now;
                    if (currentDomainUserId.HasValue)
                    {
                        entry.Entity.UpdatedByUserId = currentDomainUserId.Value;
                    }

                    // Check if entity is being inactivated (soft-deleted)
                    // This happens when InactivatedDateTime is set but was previously null
                    var originalInactivatedDateTime = entry.Property(nameof(AuditableEntityBase.InactivatedDateTime)).OriginalValue as DateTimeOffset?;
                    if (!originalInactivatedDateTime.HasValue && entry.Entity.InactivatedDateTime.HasValue)
                    {
                        // Entity is being inactivated
                        if (currentDomainUserId.HasValue)
                        {
                            entry.Entity.InactivatedByUserId = currentDomainUserId.Value;
                        }
                    }
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }


    private async Task<int?> GetCurrentDomainUserIdAsync(CancellationToken ct)
    {

        if (_hasResolvedCurrentDomainUserId)
        {
            return _cachedCurrentDomainUserId;
        }

        _hasResolvedCurrentDomainUserId = true;

        if (_currentUserService == null)
        {
            _cachedCurrentDomainUserId = null;
            return null;
        }

        try
        {
            var identityUserId = _currentUserService.GetCurrentUserId();

            var domainUserId = await DomainUsers
                .AsNoTracking()
                .Where(u => u.IdentityUserId == identityUserId)
                .Select(u => (int?)u.Id)
                .SingleOrDefaultAsync(ct);


            if (domainUserId == null)
            {
                throw new InvalidOperationException(
                    $"No Domain User found for the current authenticated Identity user (IdentityUserId: {identityUserId}). " +
                    "Domain User must be provisioned by an administrator before performing operations that require audit tracking.");
            }

            _cachedCurrentDomainUserId = domainUserId.Value;
            return _cachedCurrentDomainUserId;
        }
        catch (UnauthorizedAccessException)
        {
            _cachedCurrentDomainUserId = null;
            return null;
        }
    }

}
