using AutoMapper;
using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class MaterialProfile : Profile
{
    public MaterialProfile()
    {
        CreateMap<Material, MaterialDto>();

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateMaterialRequestDto, Material>()
            .ForMember(dest => dest.StockLots, opt => opt.Ignore());

        // Update DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<UpdateMaterialRequestDto, Material>()
            .ForMember(dest => dest.StockLots, opt => opt.Ignore());
    }
}

