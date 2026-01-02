using CncApp.Domain.Entities;

namespace CncApp.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Domain User operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adds a new Domain User to the repository.
    /// </summary>
    /// <param name="user">The Domain User entity to add.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(User user, CancellationToken ct = default);

    /// <summary>
    /// Saves all pending changes to the database.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task SaveChangesAsync(CancellationToken ct = default);
}

