using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Shift>> ListActiveAsync(CancellationToken ct = default);
    Task<List<Shift>> ListAllAsync(CancellationToken ct = default);
    Task<List<Shift>> ListProductionAsync(CancellationToken ct = default);
    Task<Shift?> GetRunningShiftForMachineAsync(int machineId, CancellationToken ct = default);
    Task<Shift?> GetRunningShiftWithContextAsync(int shiftId, CancellationToken ct = default);
    Task<List<Shift>> ListRunningByOperatorAsync(int operatorId, CancellationToken ct = default);
    Task<List<Shift>> ListClosedByOperatorAsync(int operatorId, CancellationToken ct = default);
    Task<(List<Shift> Items, int TotalCount)> SearchByOperatorAsync(int operatorId, ShiftLogSearchRequestDto request, CancellationToken ct = default);
    Task<List<Shift>> ListOpenWithContextAsync(CancellationToken ct = default);
    Task<List<Shift>> ListStartedTodayAsync(DateOnly today, CancellationToken ct = default);
    Task AddAsync(Shift shift, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

