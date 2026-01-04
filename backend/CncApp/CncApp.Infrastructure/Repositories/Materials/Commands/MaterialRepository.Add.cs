using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository : IMaterialRepository
{
    public async Task AddAsync(Material material, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

