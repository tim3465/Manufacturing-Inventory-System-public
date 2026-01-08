using AutoMapper;
using CncApp.Application.Dtos.Parts;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class PartProfile : Profile
{
    public PartProfile()
    {
        CreateMap<Part, PartDto>();

        CreateMap<CreatePartRequestDto, Part>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());

        CreateMap<UpdatePartRequestDto, Part>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());
    }
}

