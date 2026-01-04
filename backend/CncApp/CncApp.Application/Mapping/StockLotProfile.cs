using AutoMapper;
using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class StockLotProfile : Profile
{
    public StockLotProfile()
    {
        // TODO: Add CreateMap<StockLot, StockLotDto>()
        // TODO: Add CreateMap<CreateStockLotRequestDto, StockLot>() with navigation property ignore
    }
}
