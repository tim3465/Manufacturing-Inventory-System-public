using AutoMapper;
using CncApp.Application.Dtos.Materials;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    public async Task<int> CreateAsync(CreateMaterialRequestDto dto, CancellationToken ct = default)
    {
        var material = _mapper.Map<Material>(dto);

        await _materialRepository.AddAsync(material, ct);
        await _materialRepository.SaveChangesAsync(ct);

        return material.Id;
    }
}

