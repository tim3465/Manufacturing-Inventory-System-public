using AutoMapper;

using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IMapper _mapper;

    public ShiftService(IShiftRepository shiftRepository, IMapper mapper)
    {
        _shiftRepository = shiftRepository;
        _mapper = mapper;
    }
}

