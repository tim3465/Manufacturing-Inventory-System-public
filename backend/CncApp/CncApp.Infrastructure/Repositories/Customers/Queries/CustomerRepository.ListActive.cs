using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class CustomerRepository : ICustomerRepository
{
    public async Task<List<Customer>> ListActiveAsync(CancellationToken ct = default)
    {
        return await _context.Customers
            .Where(c => !c.InactivatedDateTime.HasValue)
            .ToListAsync(ct);
    }
}
