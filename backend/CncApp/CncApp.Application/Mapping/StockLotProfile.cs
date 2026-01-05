using AutoMapper;
using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class StockLotProfile : Profile
{
    public StockLotProfile()
    {
        CreateMap<StockLot, StockLotDto>();

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateStockLotRequestDto, StockLot>()
            .ForMember(dest => dest.StockLotAdjustments, opt => opt.Ignore());

        // Update DTO maps only client-provided fields; AmountOfBars is excluded (metadata only).
        CreateMap<UpdateStockLotRequestDto, StockLot>()
            .ForMember(dest => dest.AmountOfBars, opt => opt.Ignore())
            .ForMember(dest => dest.StockLotAdjustments, opt => opt.Ignore());
    }
}

