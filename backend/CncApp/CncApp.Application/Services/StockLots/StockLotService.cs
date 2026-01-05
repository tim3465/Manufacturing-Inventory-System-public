using AutoMapper;

using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.StockLots;

public partial class StockLotService
{
    private readonly IStockLotRepository _stockLotRepository;
    private readonly IMapper _mapper;

    public StockLotService(IStockLotRepository stockLotRepository, IMapper mapper)
    {
        _stockLotRepository = stockLotRepository;
        _mapper = mapper;
    }
}

