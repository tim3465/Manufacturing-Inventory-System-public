using AutoMapper;
using CncApp.Application.Dtos.Materials;

namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    public async Task<List<MaterialDto>> ListAllAsync(CancellationToken ct = default)
    {
        var materials = await _materialRepository.ListAllAsync(ct);
        return _mapper.Map<List<MaterialDto>>(materials);
    }
}

