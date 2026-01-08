using AutoMapper;
using CncApp.Application.Dtos.Parts;

namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    public async Task<PartDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var part = await _partRepository.GetByIdAsync(id, ct);
        if (part == null)
            return null;

        return _mapper.Map<PartDto>(part);
    }
}

