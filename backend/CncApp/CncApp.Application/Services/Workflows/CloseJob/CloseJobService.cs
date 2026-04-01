using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Shifts;

namespace CncApp.Application.Services.Workflows.CloseJob;

public partial class CloseJobService
{
    private readonly ShiftService _shiftService;
    private readonly IJobRepository _jobRepository;
    private readonly ITransactionManager _transactionManager;

    public CloseJobService(
        ShiftService shiftService,
        IJobRepository jobRepository,
        ITransactionManager transactionManager)
    {
        _shiftService = shiftService;
        _jobRepository = jobRepository;
        _transactionManager = transactionManager;
    }
}
