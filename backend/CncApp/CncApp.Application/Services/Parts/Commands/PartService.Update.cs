using AutoMapper;
using CncApp.Application.Dtos.Parts;

namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    public async Task<PartDto?> UpdateAsync(int id, UpdatePartRequestDto dto, CancellationToken ct = default)
    {
        var part = await _partRepository.GetByIdAsync(id, ct);
        if (part == null)
            return null;

        // Update metadata only - ApproxPartCycleTime, CheckPerPart
        if (dto.ApproxPartCycleTime.HasValue)
        {
            part.ApproxPartCycleTime = dto.ApproxPartCycleTime.Value;
        }

        if (dto.CheckPerPart.HasValue)
        {
            part.CheckPerPart = dto.CheckPerPart.Value;
        }

        await _partRepository.UpdateAsync(part, ct);
        await _partRepository.SaveChangesAsync(ct);

        return _mapper.Map<PartDto>(part);
    }
}

