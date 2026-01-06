using CncApp.Application.Services.Machines;
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
        services.AddScoped<UserService>();
        services.AddScoped<StockLotService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}


