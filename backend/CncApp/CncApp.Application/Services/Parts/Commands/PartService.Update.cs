using CncApp.Application.Dtos.Parts;

namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    public async Task<PartDto?> UpdateAsync(int id, UpdatePartRequestDto dto, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

