using AutoMapper;
using CncApp.Application.Dtos.Machines;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Machines;

public class MachineService
{
    private readonly IMachineRepository _machineRepository;
    private readonly IMapper _mapper;

    public MachineService(IMachineRepository machineRepository, IMapper mapper)
    {
        _machineRepository = machineRepository;
        _mapper = mapper;
    }

    public async Task<int> CreateAsync(CreateMachineRequestDto dto, CancellationToken ct = default)
    {
        var machine = _mapper.Map<Machine>(dto);

        await _machineRepository.AddAsync(machine, ct);
        await _machineRepository.SaveChangesAsync(ct);

        return machine.Id;
    }

    public async Task<MachineDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var machine = await _machineRepository.GetByIdAsync(id, ct);
        if (machine == null)
            return null;

        return _mapper.Map<MachineDto>(machine);
    }

    public async Task<List<MachineDto>> ListAsync(CancellationToken ct = default)
    {
        var machines = await _machineRepository.ListAsync(ct);
        return _mapper.Map<List<MachineDto>>(machines);
    }

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

