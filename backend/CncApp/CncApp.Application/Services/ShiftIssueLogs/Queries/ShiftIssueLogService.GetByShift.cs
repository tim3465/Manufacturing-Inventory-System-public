using CncApp.Application.Dtos.ShiftIssueLogs;

namespace CncApp.Application.Services.ShiftIssueLogs;

public partial class ShiftIssueLogService
{
    public async Task<List<ShiftIssueLogResultDto>> GetByShiftAsync(int shiftId, CancellationToken ct = default)
    {
        var logs = await _shiftIssueLogRepository.ListByShiftAsync(shiftId, ct);
        return _mapper.Map<List<ShiftIssueLogResultDto>>(logs);
    }
}
