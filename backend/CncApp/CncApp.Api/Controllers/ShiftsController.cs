using CncApp.Application.Dtos.Shifts;
using CncApp.Application.Services.Shifts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing shifts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly ShiftService _shiftService;

    public ShiftsController(ShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    // TODO: Add endpoints following ledger table patterns
    // POST /api/shifts - Create
    // GET /api/shifts/{id} - Get by ID
    // GET /api/shifts/job/{jobId} - List by Job ID
}

