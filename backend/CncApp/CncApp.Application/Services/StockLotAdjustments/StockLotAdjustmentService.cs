using AutoMapper;

using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.StockLotAdjustments;

public partial class StockLotAdjustmentService
{
    private readonly IStockLotAdjustmentRepository _stockLotAdjustmentRepository;
    private readonly IStockLotRepository _stockLotRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IMapper _mapper;

    public StockLotAdjustmentService(
        IStockLotAdjustmentRepository stockLotAdjustmentRepository,
        IStockLotRepository stockLotRepository,
        ITransactionManager transactionManager,
        IMapper mapper)
    {
        _stockLotAdjustmentRepository = stockLotAdjustmentRepository;
        _stockLotRepository = stockLotRepository;
        _transactionManager = transactionManager;
        _mapper = mapper;
    }
}
