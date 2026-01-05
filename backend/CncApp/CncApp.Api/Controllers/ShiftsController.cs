using CncApp.Application.Services.Shifts;

using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly ShiftService _shiftService;

    public ShiftsController(ShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    // TODO: add actions
}

