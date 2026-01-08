using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Parts;
using CncApp.Infrastructure.Persistence;
using CncApp.Infrastructure.Repositories;
using CncApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CncApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register Repositories
        services.AddScoped<IMachineRepository, MachineRepository>();
        services.AddScoped<IMaterialRepository, MaterialRepository>();
        services.AddScoped<IPartRepository, PartRepository>();  
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStockLotRepository, StockLotRepository>();
        services.AddScoped<IStockLotAdjustmentRepository, StockLotAdjustmentRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();

        // Register Current User Service
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register Identity Provisioning Service
        services.AddScoped<IIdentityProvisioningService, IdentityProvisioningService>();

        return services;
    }
}



