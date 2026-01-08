using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public Task<List<ShiftResultDto>> ListActiveAsync(CancellationToken ct = default) =>
        throw new NotImplementedException();
}


