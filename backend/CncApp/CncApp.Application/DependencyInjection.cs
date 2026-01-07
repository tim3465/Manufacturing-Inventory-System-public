using CncApp.Application.Services.Machines;
using CncApp.Application.Services.Materials;
using CncApp.Application.Services.StockLotAdjustments;
using CncApp.Application.Services.StockLots;
using CncApp.Application.Services.Users;
using Microsoft.Extensions.DependencyInjection;

namespace CncApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Application Services
        services.AddScoped<MachineService>();
        services.AddScoped<MaterialService>();
        services.AddScoped<UserService>();
        services.AddScoped<StockLotService>();
        services.AddScoped<StockLotAdjustmentService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}


