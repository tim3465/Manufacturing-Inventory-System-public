using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Dtos.OrderPlanning;
using CncApp.Application.Dtos.Orders;

namespace CncApp.Application.Services.Workflows.OrderPlanning;

public partial class OrderPlanningService
{
    public async Task<CreateOrderWithJobsResponseDto> CreateOrderWithJobsAsync(
        CreateOrderWithJobsRequestDto dto,
        CancellationToken ct = default)
    {
        await _transactionManager.BeginTransactionAsync(ct);

        try
        {
            var orderId = await _orderService.CreateAsync(
                new CreateOrderRequestDto
                {
                    CustomerId = dto.CustomerId,
                    PartId = dto.PartId,
                    PartAmountRequested = dto.PartAmountRequested,
                    PartsPerBar = dto.PartsPerBar
                }, ct);

            var jobIds = new List<int>();

            foreach (var jobDto in dto.Jobs)
            {
                var jobId = await _jobService.CreateAsync(
                    new CreateJobRequestDto
                    {
                        OrderId = orderId,
                        StockLotId = jobDto.StockLotId,
                        MachineId = jobDto.MachineId,
                        PartAmountPlanned = jobDto.PartAmountPlanned,
                        BarAmountPlanned = jobDto.BarAmountPlanned,
                        BarCycleTime = jobDto.BarCycleTime,
                        BarsInJob = jobDto.BarsInJob,
                        EstimatedPartsPerBar = jobDto.EstimatedPartsPerBar,
                        DueDate = jobDto.DueDate
                    }, ct);

                jobIds.Add(jobId);
            }

            await _transactionManager.CommitTransactionAsync(ct);

            return new CreateOrderWithJobsResponseDto
            {
                OrderId = orderId,
                JobIds = jobIds
            };
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
