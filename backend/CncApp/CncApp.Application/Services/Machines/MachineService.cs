using AutoMapper;
using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Machines;

public partial class MachineService
{
    private readonly IMachineRepository _machineRepository;
    private readonly IMapper _mapper;

    public MachineService(IMachineRepository machineRepository, IMapper mapper)
    {
        _machineRepository = machineRepository;
        _mapper = mapper;
    }
}

