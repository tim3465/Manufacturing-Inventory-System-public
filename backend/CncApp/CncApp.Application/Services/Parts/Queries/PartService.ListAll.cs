using AutoMapper;
using CncApp.Application.Dtos.Parts;

namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    public async Task<List<PartDto>> ListAllAsync(CancellationToken ct = default)
    {
        var parts = await _partRepository.ListAllAsync(ct);
        return _mapper.Map<List<PartDto>>(parts);
    }
}

