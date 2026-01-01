using AutoMapper;
using CncApp.Application.Dtos.Machines;

namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    public async Task<List<MachineDto>> ListActiveAsync(CancellationToken ct = default)
    {
        var machines = await _machineRepository.ListActiveAsync(ct);
        return _mapper.Map<List<MachineDto>>(machines);
    }
}

