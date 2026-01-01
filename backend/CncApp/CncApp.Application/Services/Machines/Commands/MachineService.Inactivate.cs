namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _machineRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _machineRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}


