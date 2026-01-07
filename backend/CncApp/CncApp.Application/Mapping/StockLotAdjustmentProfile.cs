using AutoMapper;
using CncApp.Application.Dtos.StockLotAdjustments;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class StockLotAdjustmentProfile : Profile
{
    public StockLotAdjustmentProfile()
    {
        CreateMap<StockLotAdjustment, StockLotAdjustmentDto>();

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateStockLotAdjustmentRequestDto, StockLotAdjustment>()
            .ForMember(dest => dest.StockLot, opt => opt.Ignore());
    }
}

