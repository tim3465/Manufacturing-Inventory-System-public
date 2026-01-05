using CncApp.Application.Services.StockLots;

using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockLotsController : ControllerBase
{
    private readonly StockLotService _stockLotService;

    public StockLotsController(StockLotService stockLotService)
    {
        _stockLotService = stockLotService;
    }

    // TODO: add actions
}

