using CncApp.Application.Dtos.ShiftIssueLogs;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.ShiftIssueLogs;

public partial class ShiftIssueLogService
{
    /// <summary>
    /// Creates a shift issue log and applies its scrap/downtime to the parent
    /// Shift's cached totals, wrapped in its own transaction.
    /// Use this entrypoint from CRUD controllers.
    /// Workflows that already own a transaction should call
    /// <see cref="CreateWithinTransactionAsync"/> instead.
    /// </summary>
    public async Task<int> CreateAsync(CreateShiftIssueLogRequestDto dto, CancellationToken ct = default)
    {
        await _transactionManager.BeginTransactionAsync(ct);

        try
        {
            var id = await CreateWithinTransactionAsync(dto, ct);
            await _transactionManager.CommitTransactionAsync(ct);
            return id;
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Core logic: creates a shift issue log and applies its scrap/downtime to
    /// the parent Shift's cached totals.
    /// <para>
    /// <b>Caller MUST already have an active transaction.</b>
    /// Do not call this from controllers; use <see cref="CreateAsync"/> instead.
    /// </para>
    /// </summary>
    // Not tested directly — exercised through CreateAsync (controller path)
    // and future workflow paths.
    internal async Task<int> CreateWithinTransactionAsync(CreateShiftIssueLogRequestDto dto, CancellationToken ct = default)
    {
        var shiftIssueLog = _mapper.Map<ShiftIssueLog>(dto);

        await _shiftIssueLogRepository.AddAsync(shiftIssueLog, ct);
        await _shiftIssueLogRepository.SaveChangesAsync(ct);

        // Shift.Scrap and Shift.Downtime are cached totals derived from issue logs.
        // Every issue log must keep them in sync.
        var shift = await _shiftRepository.GetByIdAsync(dto.ShiftId, ct);
        shift!.Scrap += dto.ScrapQuantity;

        if (dto.Downtime.HasValue)
        {
            shift.Downtime = (shift.Downtime ?? TimeSpan.Zero) + dto.Downtime.Value;
        }

        await _shiftRepository.SaveChangesAsync(ct);

        return shiftIssueLog.Id;
    }
}
