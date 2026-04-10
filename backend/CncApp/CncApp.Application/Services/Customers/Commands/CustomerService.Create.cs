using CncApp.Application.Dtos.Customers;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Customers;

public partial class CustomerService
{
    public async Task<int> CreateAsync(CreateCustomerRequestDto dto, CancellationToken ct = default)
    {
        var customer = _mapper.Map<Customer>(dto);

        await _customerRepository.AddAsync(customer, ct);
        await _customerRepository.SaveChangesAsync(ct);

        return customer.Id;
    }
}
