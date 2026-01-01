using AutoMapper;
using CncApp.Application.Dtos.Machines;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class MachineProfile : Profile
{
    public MachineProfile()
    {
        CreateMap<Machine, MachineDto>();

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateMachineRequestDto, Machine>()
            .ForMember(dest => dest.Jobs, opt => opt.Ignore());
    }
}

