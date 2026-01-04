using CncApp.Application.Dtos.StockLots;
using CncApp.Application.Services.StockLots;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing stock lots.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StockLotsController : ControllerBase
{
    private readonly StockLotService _stockLotService;

    public StockLotsController(StockLotService stockLotService)
    {
        _stockLotService = stockLotService;
    }

    // TODO: Add endpoints following MachinesController pattern
    // POST /api/stocklots - Create
    // GET /api/stocklots - List active
    // GET /api/stocklots/all - List all
    // GET /api/stocklots/{id} - Get by ID
    // DELETE /api/stocklots/{id} - Inactivate
}
