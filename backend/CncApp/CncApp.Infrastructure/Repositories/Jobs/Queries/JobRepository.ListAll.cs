using CncApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository
{
    public async Task<List<Job>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.Jobs.ToListAsync(ct);
    }
}

