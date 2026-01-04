using CncApp.Application.Dtos.Materials;
using CncApp.Application.Services.Materials;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing materials.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly MaterialService _materialService;

    public MaterialsController(MaterialService materialService)
    {
        _materialService = materialService;
    }

    // TODO: Add endpoints following MachinesController pattern
    // POST /api/materials - Create
    // GET /api/materials - List active
    // GET /api/materials/all - List all
    // GET /api/materials/{id} - Get by ID
    // DELETE /api/materials/{id} - Inactivate
}

