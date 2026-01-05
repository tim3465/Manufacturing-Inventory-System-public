using CncApp.Application.Interfaces.Repositories;

using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class JobRepository : IJobRepository
{
    private readonly AppDbContext _context;

    public JobRepository(AppDbContext context)
    {
        _context = context;
    }
}

