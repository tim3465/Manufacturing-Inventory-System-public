namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _shiftRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _shiftRepository.SaveChangesAsync(ct);
        }

        return result;
    }
}


