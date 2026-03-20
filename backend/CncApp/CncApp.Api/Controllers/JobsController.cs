using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Services.Jobs;
using CncApp.Application.Services.Users;
using CncApp.Application.Services.Workflows.StartJob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CncApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly JobService _jobService;
    private readonly StartJobService _startJobService;
    private readonly UserService _userService;

    public JobsController(JobService jobService, StartJobService startJobService, UserService userService)
    {
        _jobService = jobService;
        _startJobService = startJobService;
        _userService = userService;
    }

    // Conventions:
    // - All deletes are soft deletes via PATCH /{id}/inactivate.
    // - GET /all endpoints are Admin only and include inactive records.
    // - Most resources allow anonymous read access; Users requires authentication.

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<JobDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<JobDto>>> ListAsync(CancellationToken ct = default)
    {
        var jobs = await _jobService.ListActiveAsync(ct);
        return Ok(jobs);
    }

    [HttpGet("{id:int}", Name = "GetJob")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDto>> GetAsync(int id, CancellationToken ct = default)
    {
        var job = await _jobService.GetAsync(id, ct);
        if (job == null)
        {
            return NotFound();
        }

        return Ok(job);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<JobDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<JobDto>>> ListAllAsync(CancellationToken ct = default)
    {
        var jobs = await _jobService.ListAllAsync(ct);
        return Ok(jobs);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobDto>> CreateAsync(
        [FromBody] CreateJobRequestDto dto,
        CancellationToken ct = default)
    {
        var id = await _jobService.CreateAsync(dto, ct);
        var job = await _jobService.GetAsync(id, ct);
        return CreatedAtRoute(routeName: "GetJob", routeValues: new { id }, value: job);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDto>> UpdateAsync(
        int id,
        [FromBody] UpdateJobRequestDto dto,
        CancellationToken ct = default)
    {
        var job = await _jobService.UpdateAsync(id, dto, ct);
        if (job == null)
        {
            return NotFound();
        }

        return Ok(job);
    }

    [HttpPatch("{id:int}/inactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InactivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _jobService.InactivateAsync(id, null, ct);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("production")]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(typeof(List<JobProductionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<JobProductionDto>>> ListProductionAsync(CancellationToken ct = default)
    {
        var jobs = await _jobService.ListProductionAsync(ct);
        return Ok(jobs);
    }

    [HttpPost("{id:int}/start")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(StartJobResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StartJobResponseDto>> StartJobAsync(
        int id,
        [FromBody] StartJobRequestDto dto,
        CancellationToken ct = default)
    {
        var result = await _startJobService.StartJobAsync(id, dto, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
    [HttpPatch("{id:int}/assign-stocklot")]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignStockLotAsync(int id, [FromBody] AssignStockLotRequestDto dto, CancellationToken ct = default)
    {
        var success = await _jobService.AssignStockLotAsync(id, dto, ct);
        return success ? NoContent() : NotFound();

    }

    [HttpGet("my-jobs")]
    [Authorize(Roles = "Machinist,Admin")]
    [ProducesResponseType(typeof(List<MyJobDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MyJobDto>>> ListMyJobsAsync(CancellationToken ct = default)
    {
        var operator_ = await _userService.GetCurrentUserAsync(ct);
        var jobs = await _jobService.ListMyJobsAsync(operator_.Id, ct);
        return Ok(jobs);
    }
}

