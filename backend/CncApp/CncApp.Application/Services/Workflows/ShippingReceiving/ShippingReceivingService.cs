using CncApp.Application.Interfaces;
using CncApp.Application.Services.Materials;
using CncApp.Application.Services.StockLotAdjustments;
using CncApp.Application.Services.StockLots;

namespace CncApp.Application.Services.Workflows.ShippingReceiving;

public partial class ShippingReceivingService
{
    private readonly MaterialService _materialService;
    private readonly StockLotService _stockLotService;
    private readonly StockLotAdjustmentService _stockLotAdjustmentService;
    private readonly ITransactionManager _transactionManager;

    public ShippingReceivingService(
        MaterialService materialService,
        StockLotService stockLotService,
        StockLotAdjustmentService stockLotAdjustmentService,
        ITransactionManager transactionManager)
    {
        _materialService = materialService;
        _stockLotService = stockLotService;
        _stockLotAdjustmentService = stockLotAdjustmentService;
        _transactionManager = transactionManager;
    }
}
