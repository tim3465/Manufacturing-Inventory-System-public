using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Job>> ListActiveAsync(CancellationToken ct = default);
    Task<List<Job>> ListAllAsync(CancellationToken ct = default);
    Task<List<Job>> ListActiveWithShiftsAsync(CancellationToken ct = default);
    Task<List<Job>> ListWithShiftsByOperatorAsync(int operatorId, CancellationToken ct = default);
    Task<Job?> GetActiveJobByMachineAsync(int machineId, CancellationToken ct = default);
    Task AddAsync(Job job, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

