using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<JobDto?> UpdateAsync(int id, UpdateJobRequestDto dto, CancellationToken ct = default)
    {
        var job = await _jobRepository.GetByIdAsync(id, ct);
        if (job == null)
        {
            return null;
        }

        // Metadata-only (planning fields only)
        if (dto.MachineId.HasValue)
        {
            job.MachineId = dto.MachineId.Value;
        }

        if (dto.StockLotId.HasValue)
        {
            job.StockLotId = dto.StockLotId.Value;
        }

        if (dto.PartAmountPlanned.HasValue)
        {
            job.PartAmountPlanned = dto.PartAmountPlanned.Value;
        }

        if (dto.BarAmountPlanned.HasValue)
        {
            job.BarAmountPlanned = dto.BarAmountPlanned.Value;
        }

        if (dto.BarCycleTime.HasValue)
        {
            job.BarCycleTime = dto.BarCycleTime.Value;
        }

        if (dto.BarsInJob.HasValue)
        {
            job.BarsInJob = dto.BarsInJob.Value;
        }

        if (dto.EstimatedPartsPerBar.HasValue)
        {
            job.EstimatedPartsPerBar = dto.EstimatedPartsPerBar.Value;
        }

        await _jobRepository.SaveChangesAsync(ct);

        return _mapper.Map<JobDto>(job);
    }
}

