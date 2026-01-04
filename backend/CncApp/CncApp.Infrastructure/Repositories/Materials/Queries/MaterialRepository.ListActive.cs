using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository : IMaterialRepository
{
    public async Task<List<Material>> ListActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

