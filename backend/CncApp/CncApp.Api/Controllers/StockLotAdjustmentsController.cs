using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Application.Services.StockLotAdjustments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing stock lot adjustments.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StockLotAdjustmentsController : ControllerBase
{
    private readonly StockLotAdjustmentService _stockLotAdjustmentService;

    public StockLotAdjustmentsController(StockLotAdjustmentService stockLotAdjustmentService)
    {
        _stockLotAdjustmentService = stockLotAdjustmentService;
    }

    // TODO: Add endpoints following ledger table patterns
    // POST /api/stocklotadjustments - Create
    // GET /api/stocklotadjustments/{id} - Get by ID
    // GET /api/stocklotadjustments/stocklot/{stockLotId} - List by Stock Lot ID
}

