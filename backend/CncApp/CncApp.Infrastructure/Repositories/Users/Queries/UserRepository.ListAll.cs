using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository
{
    public Task<List<User>> ListAllAsync(CancellationToken ct = default)
    {
        return _context.DomainUsers
            .AsNoTracking()
            .OrderBy(u => u.UserName)
            .ToListAsync(ct);
    }
}

