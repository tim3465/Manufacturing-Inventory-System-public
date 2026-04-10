using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class CustomerRepository : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Customers.FindAsync(new object[] { id }, ct);
    }
}
