using AutoMapper;
using CncApp.Application.Dtos.Materials;

namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    public async Task<MaterialDto?> UpdateAsync(int id, UpdateMaterialRequestDto dto, CancellationToken ct = default)
    {
        var material = await _materialRepository.GetByIdAsync(id, ct);
        if (material == null)
            return null;

        // Update metadata only - HeatNumber, MaterialName
        _mapper.Map(dto, material);

        await _materialRepository.SaveChangesAsync(ct);

        return _mapper.Map<MaterialDto>(material);
    }
}

