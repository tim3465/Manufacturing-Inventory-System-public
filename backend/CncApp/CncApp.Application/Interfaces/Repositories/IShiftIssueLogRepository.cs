using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IShiftIssueLogRepository
{
    Task AddAsync(ShiftIssueLog shiftIssueLog, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
