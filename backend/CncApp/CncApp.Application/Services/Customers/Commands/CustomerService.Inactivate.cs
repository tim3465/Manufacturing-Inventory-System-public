namespace CncApp.Application.Services.Customers;

public partial class CustomerService
{
    public async Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        var result = await _customerRepository.InactivateAsync(id, inactivatedByUserId, ct);
        if (result)
        {
            await _customerRepository.SaveChangesAsync(ct);
        }
        return result;
    }
}
