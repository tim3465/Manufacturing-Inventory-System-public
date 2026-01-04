using CncApp.Application.Dtos.Parts;
using CncApp.Application.Services.Parts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing parts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly PartService _partService;

    public PartsController(PartService partService)
    {
        _partService = partService;
    }

    // TODO: Add endpoints following MachinesController pattern
    // POST /api/parts - Create
    // GET /api/parts - List active
    // GET /api/parts/all - List all
    // GET /api/parts/{id} - Get by ID
    // DELETE /api/parts/{id} - Inactivate
}

