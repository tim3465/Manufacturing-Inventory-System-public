using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository : IUserRepository
{
    /// <inheritdoc />
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.DomainUsers.AddAsync(user, ct);
    }
}

