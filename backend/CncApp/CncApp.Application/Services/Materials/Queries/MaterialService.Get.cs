using AutoMapper;
using CncApp.Application.Dtos.Materials;

namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    public async Task<MaterialDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var material = await _materialRepository.GetByIdAsync(id, ct);
        if (material == null)
            return null;

        return _mapper.Map<MaterialDto>(material);
    }
}

