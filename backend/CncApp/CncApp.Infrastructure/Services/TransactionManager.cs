using CncApp.Application.Interfaces;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace CncApp.Infrastructure.Services;

public class TransactionManager : ITransactionManager
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public TransactionManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _currentTransaction = await _context.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(ct);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
