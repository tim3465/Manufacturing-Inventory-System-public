using CncApp.Application.Interfaces.Repositories;

using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftIssueLogRepository : IShiftIssueLogRepository
{
    private readonly AppDbContext _context;

    public ShiftIssueLogRepository(AppDbContext context)
    {
        _context = context;
    }
}
