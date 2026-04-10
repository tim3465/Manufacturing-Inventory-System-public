using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<List<MyJobListItemDto>> ListMyJobsAsync(int operatorId, CancellationToken ct = default)
    {
        var jobs = await _jobRepository.ListByOperatorAsync(operatorId, ct);

        return jobs.Select(j => new MyJobListItemDto
        {
            Id = j.Id,
            JobNumber = j.Id.ToString(),
            PartNumber = j.Order?.Part?.PartNumber ?? string.Empty,
            PartName = j.Order?.Part?.PartName ?? string.Empty,
            MachineName = j.Machine?.SerialNumber ?? string.Empty,
            EndedDateTime = j.EndedDateTime
        }).ToList();
    }
}
