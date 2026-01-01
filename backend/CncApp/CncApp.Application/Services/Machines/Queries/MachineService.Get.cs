using AutoMapper;
using CncApp.Application.Dtos.Machines;

namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    public async Task<MachineDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var machine = await _machineRepository.GetByIdAsync(id, ct);
        if (machine == null)
            return null;

        return _mapper.Map<MachineDto>(machine);
    }
}


