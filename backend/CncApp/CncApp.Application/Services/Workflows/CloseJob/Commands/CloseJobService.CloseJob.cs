using CncApp.Application.Dtos.CloseJob;

namespace CncApp.Application.Services.Workflows.CloseJob;

public partial class CloseJobService
{
    public async Task<CloseJobResponseDto> CloseJobAsync(
        CloseJobRequestDto dto, int operatorId, CancellationToken ct = default)
    {
        await _transactionManager.BeginTransactionAsync(ct);

        try
        {
            var shiftClosed = await _shiftService.CloseShiftAsync(dto.ShiftId, operatorId, dto.ShiftData, ct);
            if (!shiftClosed)
                throw new InvalidOperationException("Shift not found or already inactivated.");

            var job = await _jobRepository.GetByIdAsync(dto.JobId, ct);
            if (job == null || job.InactivatedDateTime.HasValue)
                throw new InvalidOperationException("Job not found or has been inactivated.");

            job.Close();
            await _jobRepository.SaveChangesAsync(ct);

            await _transactionManager.CommitTransactionAsync(ct);

            return new CloseJobResponseDto
            {
                JobId = job.Id,
                ShiftId = dto.ShiftId,
                JobEndedDateTime = job.EndedDateTime!.Value
            };
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
