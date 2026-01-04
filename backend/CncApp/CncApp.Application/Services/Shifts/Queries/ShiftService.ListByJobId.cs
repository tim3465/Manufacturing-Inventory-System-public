using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    // TODO: clarify - Ledger tables typically use command-centric methods
    // Consider if this should be ListByJobIdAsync or similar pattern
    public async Task<List<ShiftDto>> ListByJobIdAsync(int jobId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

