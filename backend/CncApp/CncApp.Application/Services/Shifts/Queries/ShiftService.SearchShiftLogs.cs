using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Shifts;

public partial class ShiftService
{
    public async Task<ShiftLogSearchResultDto> SearchShiftLogsAsync(
        int operatorId, ShiftLogSearchRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _shiftRepository.SearchByOperatorAsync(operatorId, request, ct);

        var dtos = items.Select(s => new ShiftLogDto
        {
            Id = s.Id,
            MachineSerialNumber = s.Job.Machine?.SerialNumber ?? string.Empty,
            JobNumber = s.JobId.ToString(),
            PartNumber = s.Job.Order?.Part?.PartNumber ?? string.Empty,
            StartTime = s.StartTime,
            StopTime = s.StopTime,
            PartsMade = s.PartsMade,
            Scrap = s.Scrap
        }).ToList();

        return new ShiftLogSearchResultDto
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
