using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Materials;
using CncApp.Application.Services.StockLotAdjustments;
using CncApp.Application.Services.StockLots;

namespace CncApp.Application.Services.Workflows.ShippingReceiving;

public partial class ShippingReceivingService
{
    private readonly MaterialService _materialService;
    private readonly StockLotService _stockLotService;
    private readonly StockLotAdjustmentService _stockLotAdjustmentService;
    private readonly IStockLotRepository _stockLotRepository;
    private readonly ITransactionManager _transactionManager;

    public ShippingReceivingService(
        MaterialService materialService,
        StockLotService stockLotService,
        StockLotAdjustmentService stockLotAdjustmentService,
        IStockLotRepository stockLotRepository,
        ITransactionManager transactionManager)
    {
        _materialService = materialService;
        _stockLotService = stockLotService;
        _stockLotAdjustmentService = stockLotAdjustmentService;
        _stockLotRepository = stockLotRepository;
        _transactionManager = transactionManager;
    }
}
