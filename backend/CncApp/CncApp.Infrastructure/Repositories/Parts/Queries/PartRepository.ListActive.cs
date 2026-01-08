using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task<List<Part>> ListActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

