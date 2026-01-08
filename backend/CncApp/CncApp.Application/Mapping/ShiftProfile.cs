using AutoMapper;
using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class ShiftProfile : Profile
{
    public ShiftProfile()
    {
        CreateMap<Shift, ShiftResultDto>();
        CreateMap<CreateShiftRequestDto, Shift>();
    }
}

