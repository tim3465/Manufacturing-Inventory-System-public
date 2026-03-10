using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Customer>> ListActiveAsync(CancellationToken ct = default);
    Task<List<Customer>> ListAllAsync(CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
    Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
