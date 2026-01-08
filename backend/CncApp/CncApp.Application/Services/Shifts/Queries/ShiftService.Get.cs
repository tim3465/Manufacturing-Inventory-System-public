using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public Task<ShiftDto?> GetAsync(int id, CancellationToken ct = default) =>
        throw new NotImplementedException();
}


