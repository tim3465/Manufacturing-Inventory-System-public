using CncApp.Application.Interfaces;
using CncApp.Application.Services.Jobs;
using CncApp.Application.Services.Orders;

namespace CncApp.Application.Services.Workflows.OrderPlanning;

public partial class OrderPlanningService
{
    private readonly OrderService _orderService;
    private readonly JobService _jobService;
    private readonly ITransactionManager _transactionManager;

    public OrderPlanningService(
        OrderService orderService,
        JobService jobService,
        ITransactionManager transactionManager)
    {
        _orderService = orderService;
        _jobService = jobService;
        _transactionManager = transactionManager;
    }
}
