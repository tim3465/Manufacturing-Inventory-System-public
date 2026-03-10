using CncApp.Application.Dtos.Customers;

namespace CncApp.Application.Services.Customers;

public partial class CustomerService
{
    public async Task<CustomerDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, ct);
        if (customer == null)
            return null;

        return _mapper.Map<CustomerDto>(customer);
    }
}
