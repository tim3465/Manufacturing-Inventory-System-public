using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _context.DomainUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}

