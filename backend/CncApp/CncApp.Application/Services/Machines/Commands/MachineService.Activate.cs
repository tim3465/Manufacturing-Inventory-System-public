namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    public async Task<bool> ActivateAsync(int id, CancellationToken ct = default)
    {
        var result = await _machineRepository.ActivateAsync(id, ct);
        if (result)
        {
            await _machineRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}
