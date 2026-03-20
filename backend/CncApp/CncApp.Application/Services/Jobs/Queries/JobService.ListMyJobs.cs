using CncApp.Application.Dtos.Jobs;
using CncApp.Application.Dtos.Shifts;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<MyJobDto>> ListMyJobsAsync(int operatorId, CancellationToken ct = default)
    {
        var jobs = await _jobRepository.ListWithShiftsByOperatorAsync(operatorId, ct);

        return jobs.Select(j => new MyJobDto
        {
            Id = j.Id,
            JobNumber = j.Id.ToString(),
            PartNumber = j.Order?.Part?.PartNumber ?? string.Empty,
            PartName = j.Order?.Part?.PartName ?? string.Empty,
            MachineName = j.Machine?.SerialNumber ?? string.Empty,
            EndedDateTime = j.EndedDateTime,
            Shifts = _mapper.Map<List<ShiftDto>>(j.Shifts)
        }).ToList();
    }
}
