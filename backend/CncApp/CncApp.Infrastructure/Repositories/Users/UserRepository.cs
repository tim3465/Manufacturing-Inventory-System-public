using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
}

