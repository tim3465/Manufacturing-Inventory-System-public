using AutoMapper;

using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    private readonly IStockLotAdjustmentRepository _stockLotAdjustmentRepository;
    private readonly IMapper _mapper;

    public StockLotAdjustmentService(IStockLotAdjustmentRepository stockLotAdjustmentRepository, IMapper mapper)
    {
        _stockLotAdjustmentRepository = stockLotAdjustmentRepository;
        _mapper = mapper;
    }
}

