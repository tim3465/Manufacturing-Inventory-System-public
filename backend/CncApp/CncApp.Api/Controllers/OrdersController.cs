using CncApp.Application.Dtos.Orders;
using CncApp.Application.Services.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing orders.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    // TODO: Add endpoints following MachinesController pattern
    // POST /api/orders - Create
    // GET /api/orders - List active
    // GET /api/orders/all - List all
    // GET /api/orders/{id} - Get by ID
    // DELETE /api/orders/{id} - Inactivate
}

