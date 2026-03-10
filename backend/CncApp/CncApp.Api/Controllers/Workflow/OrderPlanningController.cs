using CncApp.Application.Dtos.OrderPlanning;
using CncApp.Application.Services.Workflows.OrderPlanning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CncApp.Api.Controllers.Workflow;

/// <summary>
/// Controller for the Order Planning workflow (creates Order + Jobs atomically).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrderPlanningController : ControllerBase
{
    private readonly OrderPlanningService _orderPlanningService;

    public OrderPlanningController(OrderPlanningService orderPlanningService)
    {
        _orderPlanningService = orderPlanningService;
    }

    /// <summary>
    /// Creates an order with one or more jobs in a single atomic transaction.
    /// </summary>
    [HttpPost("create")]
    [Authorize(Roles = "Supervisor,Admin")]
    [ProducesResponseType(typeof(CreateOrderWithJobsResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateOrderWithJobsResponseDto>> CreateOrderWithJobsAsync(
        [FromBody] CreateOrderWithJobsRequestDto dto,
        CancellationToken ct = default)
    {
        var result = await _orderPlanningService.CreateOrderWithJobsAsync(dto, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
