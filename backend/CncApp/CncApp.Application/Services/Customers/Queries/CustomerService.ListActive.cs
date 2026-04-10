using CncApp.Application.Dtos.Customers;

namespace CncApp.Application.Services.Customers;

public partial class CustomerService
{
    public async Task<List<CustomerDto>> ListActiveAsync(CancellationToken ct = default)
    {
            var customers = await _customerRepository.ListActiveAsync(ct);
            return _mapper.Map<List<CustomerDto>>(customers);
    }
}
