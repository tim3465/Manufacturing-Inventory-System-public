using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public Task<ShiftResultDto?> GetAsync(int id, CancellationToken ct = default) =>
        throw new NotImplementedException();
}


