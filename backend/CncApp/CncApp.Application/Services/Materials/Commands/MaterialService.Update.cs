using CncApp.Application.Dtos.Materials;

namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    public async Task<MaterialDto?> UpdateAsync(int id, UpdateMaterialRequestDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

