using AutoMapper;

using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IMapper _mapper;

    public ShiftService(IShiftRepository shiftRepository, IJobRepository jobRepository, IMapper mapper)
    {
        _shiftRepository = shiftRepository;
        _jobRepository = jobRepository;
        _mapper = mapper;
    }
}

