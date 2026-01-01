using AutoMapper;
using CncApp.Application.Dtos.Machines;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    public async Task<int> CreateAsync(CreateMachineRequestDto dto, CancellationToken ct = default)
    {
        var machine = _mapper.Map<Machine>(dto);

        await _machineRepository.AddAsync(machine, ct);
        await _machineRepository.SaveChangesAsync(ct);

        return machine.Id;
    }
}


