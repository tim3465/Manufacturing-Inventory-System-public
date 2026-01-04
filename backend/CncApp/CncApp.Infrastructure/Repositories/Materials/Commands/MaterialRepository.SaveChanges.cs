using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository : IMaterialRepository
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

