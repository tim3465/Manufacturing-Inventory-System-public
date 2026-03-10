using CncApp.Application.Services.Customers;
using CncApp.Application.Services.Machines;
using CncApp.Application.Services.Jobs;
using CncApp.Application.Services.Materials;
using CncApp.Application.Services.Orders;
using CncApp.Application.Services.Parts;
using CncApp.Application.Services.Shifts;
using CncApp.Application.Services.StockLotAdjustments;
using CncApp.Application.Services.StockLots;
using CncApp.Application.Services.Users;
using CncApp.Application.Services.Workflows.OrderPlanning;
using CncApp.Application.Services.Workflows.ShippingReceiving;
using Microsoft.Extensions.DependencyInjection;

namespace CncApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Application Services
        services.AddScoped<MachineService>();
        services.AddScoped<JobService>();
        services.AddScoped<MaterialService>();
        services.AddScoped<UserService>();
        services.AddScoped<StockLotService>();
        services.AddScoped<StockLotAdjustmentService>();
        services.AddScoped<PartService>();
        services.AddScoped<OrderService>();
        services.AddScoped<ShiftService>();
        services.AddScoped<CustomerService>();

        // Register Workflow Services
        services.AddScoped<ShippingReceivingService>();
        services.AddScoped<OrderPlanningService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        return services;
    }
}


