using CncApp.Application.Services.Materials;

using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly MaterialService _materialService;

    public MaterialsController(MaterialService materialService)
    {
        _materialService = materialService;
    }

    // TODO: add actions
}

