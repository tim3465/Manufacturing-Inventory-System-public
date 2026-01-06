namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _materialRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _materialRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}

