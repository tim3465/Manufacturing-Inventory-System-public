using CncApp.Application.Services.StockLotAdjustments;

using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockLotAdjustmentsController : ControllerBase
{
    private readonly StockLotAdjustmentService _stockLotAdjustmentService;

    public StockLotAdjustmentsController(StockLotAdjustmentService stockLotAdjustmentService)
    {
        _stockLotAdjustmentService = stockLotAdjustmentService;
    }

    // TODO: add actions
}

