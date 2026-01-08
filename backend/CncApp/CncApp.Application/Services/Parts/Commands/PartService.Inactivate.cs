namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _partRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _partRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}

