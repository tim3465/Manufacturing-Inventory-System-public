using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class CustomerRepository : ICustomerRepository
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var customer = await _context.Customers.FindAsync(new object[] { id }, ct);
        if (customer == null)
            return false;

        customer.Inactivate(inactivatedByUserId);

        return true;
    }
}
