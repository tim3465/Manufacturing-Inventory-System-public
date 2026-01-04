using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Services.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers;

/// <summary>
/// Controller for managing jobs.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;

    public JobsController(JobService jobService)
    {
        _jobService = jobService;
    }

    // TODO: Add endpoints following MachinesController pattern
    // POST /api/jobs - Create
    // GET /api/jobs - List active
    // GET /api/jobs/all - List all
    // GET /api/jobs/{id} - Get by ID
    // DELETE /api/jobs/{id} - Inactivate
}

