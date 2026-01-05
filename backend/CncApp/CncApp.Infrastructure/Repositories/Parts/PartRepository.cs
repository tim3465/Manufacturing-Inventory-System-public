using CncApp.Application.Interfaces.Repositories;

using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository : IPartRepository
{
    private readonly AppDbContext _context;

    public PartRepository(AppDbContext context)
    {
        _context = context;
    }
}

