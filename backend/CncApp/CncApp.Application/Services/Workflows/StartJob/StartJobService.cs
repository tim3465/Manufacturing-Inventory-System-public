using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.StockLotAdjustments;

namespace CncApp.Application.Services.Workflows.StartJob;

public partial class StartJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly StockLotAdjustmentService _stockLotAdjustmentService;
    private readonly ITransactionManager _transactionManager;

    public StartJobService(
        IJobRepository jobRepository,
        StockLotAdjustmentService stockLotAdjustmentService,
        ITransactionManager transactionManager)
    {
        _jobRepository = jobRepository;
        _stockLotAdjustmentService = stockLotAdjustmentService;
        _transactionManager = transactionManager;
    }
}
