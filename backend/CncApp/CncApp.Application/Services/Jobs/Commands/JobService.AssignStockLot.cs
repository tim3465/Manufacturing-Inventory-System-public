using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<bool> AssignStockLotAsync(int id, AssignStockLotRequestDto dto, CancellationToken ct = default)
    {
        var job = await _jobRepository.GetByIdAsync(id, ct);
        if (job is null) return false;
        job.StockLotId = dto.StockLotId;
        await _jobRepository.SaveChangesAsync(ct);
        return true;
    }
}
