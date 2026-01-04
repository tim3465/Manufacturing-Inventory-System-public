using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class ShiftRepository : IShiftRepository
{
    public async Task<Shift?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

