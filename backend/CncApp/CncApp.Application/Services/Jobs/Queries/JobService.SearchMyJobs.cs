using CncApp.Application.Dtos.Jobs;

namespace CncApp.Application.Services.Jobs;

public partial class JobService
{
    public async Task<MyJobSearchResultDto> SearchMyJobsAsync(
        int operatorId, MyJobSearchRequestDto request, CancellationToken ct = default)
    {
        var (items, totalCount) = await _jobRepository.SearchByOperatorAsync(operatorId, request, ct);

        var dtos = items.Select(j => new MyJobListItemDto
        {
            Id = j.Id,
            JobNumber = j.Id.ToString(),
            PartNumber = j.Order?.Part?.PartNumber ?? string.Empty,
            PartName = j.Order?.Part?.PartName ?? string.Empty,
            MachineName = j.Machine?.SerialNumber ?? string.Empty,
            EndedDateTime = j.EndedDateTime
        }).ToList();

        return new MyJobSearchResultDto
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
