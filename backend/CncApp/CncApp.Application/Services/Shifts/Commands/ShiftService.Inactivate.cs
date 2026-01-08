namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default) =>
        throw new NotImplementedException();
}


