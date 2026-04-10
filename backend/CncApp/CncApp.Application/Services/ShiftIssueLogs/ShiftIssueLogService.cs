using AutoMapper;

using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.ShiftIssueLogs;

public partial class ShiftIssueLogService
{
    private readonly IShiftIssueLogRepository _shiftIssueLogRepository;
    private readonly IShiftRepository _shiftRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionManager _transactionManager;
    private readonly IMapper _mapper;

    public ShiftIssueLogService(
        IShiftIssueLogRepository shiftIssueLogRepository,
        IShiftRepository shiftRepository,
        IUserRepository userRepository,
        ITransactionManager transactionManager,
        IMapper mapper)
    {
        _shiftIssueLogRepository = shiftIssueLogRepository;
        _shiftRepository = shiftRepository;
        _userRepository = userRepository;
        _transactionManager = transactionManager;
        _mapper = mapper;
    }
}
