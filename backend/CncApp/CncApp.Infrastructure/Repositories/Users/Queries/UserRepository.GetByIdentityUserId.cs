using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository : IUserRepository
{
    /// <inheritdoc />
    public async Task<User?> GetByIdentityUserIdAsync(int identityUserId, CancellationToken ct = default)
    {
        return await _context.DomainUsers
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, ct);
    }
}

