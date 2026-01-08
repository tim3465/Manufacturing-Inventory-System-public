namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

