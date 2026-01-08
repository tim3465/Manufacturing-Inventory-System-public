using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository
{
    public Task<List<User>> ListActiveAsync(CancellationToken ct = default)
    {
        return _context.DomainUsers
            .AsNoTracking()
            .Where(u => !u.InactivatedDateTime.HasValue)
            .OrderBy(u => u.UserName)
            .ToListAsync(ct);
    }
}

