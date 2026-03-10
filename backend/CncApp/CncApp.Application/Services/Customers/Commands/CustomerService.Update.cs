using CncApp.Application.Dtos.Customers;

namespace CncApp.Application.Services.Customers;

public partial class CustomerService
{
    public async Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequestDto dto, CancellationToken ct = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, ct);
        if (customer == null)
            return null;

        customer.CompanyName = dto.CompanyName;
        customer.Phone = dto.Phone;
        customer.Email = dto.Email;
        customer.Address = dto.Address;

        await _customerRepository.SaveChangesAsync(ct);

        return _mapper.Map<CustomerDto>(customer);
    }
}
