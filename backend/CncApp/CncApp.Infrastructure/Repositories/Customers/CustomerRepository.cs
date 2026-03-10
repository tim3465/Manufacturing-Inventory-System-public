using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }
}
