using CncApp.Application.Dtos.SupervisorDashboard;
using CncApp.Application.Services.Workflows.SupervisorDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers.Workflow;

/// <summary>
/// Workflow controller for supervisor dashboard metrics and operator activity.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SupervisorDashboardController : ControllerBase
{
    private readonly SupervisorDashboardService _supervisorDashboardService;

    public SupervisorDashboardController(SupervisorDashboardService supervisorDashboardService)
    {
        _supervisorDashboardService = supervisorDashboardService;
    }

    /// <summary>
    /// Returns aggregated supervisor dashboard metrics including machines running,
    /// active operators, late jobs, and per-operator production summaries.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The current supervisor dashboard snapshot.</returns>
    [HttpGet]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(typeof(SupervisorDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SupervisorDashboardDto>> GetDashboardAsync(CancellationToken ct = default)
    {
        var result = await _supervisorDashboardService.GetDashboardAsync(ct);
        return Ok(result);
    }
}
