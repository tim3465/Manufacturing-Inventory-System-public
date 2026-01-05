using CncApp.Application.Interfaces.Repositories;

using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class MaterialRepository : IMaterialRepository
{
    private readonly AppDbContext _context;

    public MaterialRepository(AppDbContext context)
    {
        _context = context;
    }
}

