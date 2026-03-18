using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Domain.Enums;

namespace CncApp.Application.Services.Workflows.StartJob;

public partial class StartJobService
{
    public async Task<StartJobResponseDto> StartJobAsync(
        int jobId,
        StartJobRequestDto dto,
        CancellationToken ct = default)
    {
        await _transactionManager.BeginTransactionAsync(ct);

        try
        {
            var job = await _jobRepository.GetByIdAsync(jobId, ct);
            if (job == null)
            {
                throw new InvalidOperationException("Job not found.");
            }

            if (job.StockLotId == null)
            {
                throw new InvalidOperationException("Job has no stock lot assigned.");
            }

            var activeJob = await _jobRepository.GetActiveJobByMachineAsync(job.MachineId, ct);
            if (activeJob != null)
            {
                throw new InvalidOperationException("Machine already has an active job.");
            }

            job.Start(dto.BarsToAdd);
            await _jobRepository.SaveChangesAsync(ct);

            var adjustmentId = await _stockLotAdjustmentService.CreateWithinTransactionAsync(
                new CreateStockLotAdjustmentRequestDto
                {
                    StockLotId = job.StockLotId.Value,
                    JobId = jobId,
                    DeltaBars = -dto.BarsToAdd,
                    Reason = StockLotAdjustmentReasonEnum.JobStart
                }, ct);

            await _transactionManager.CommitTransactionAsync(ct);

            return new StartJobResponseDto
            {
                JobId = jobId,
                StockLotAdjustmentId = adjustmentId
            };
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
