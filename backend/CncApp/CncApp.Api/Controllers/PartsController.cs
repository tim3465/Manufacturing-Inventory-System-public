using CncApp.Application.Services.Parts;

using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly PartService _partService;

    public PartsController(PartService partService)
    {
        _partService = partService;
    }

    // TODO: add actions
}

