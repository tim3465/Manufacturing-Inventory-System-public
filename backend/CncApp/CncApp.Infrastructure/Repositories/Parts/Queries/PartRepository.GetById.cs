using CncApp.Domain.Entities;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task<Part?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

