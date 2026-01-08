namespace CncApp.Infrastructure.Repositories;

public partial class UserRepository
{
    public Task<bool> InactivateAsync(int id, int? inactivatedByUserId = null, CancellationToken ct = default)
    {
        return InactivateInternalAsync(id, ct);
    }

    private async Task<bool> InactivateInternalAsync(int id, CancellationToken ct)
    {
        var user = await _context.DomainUsers.FindAsync(new object[] { id }, ct);
        if (user == null)
        {
            return false;
        }

        if (user.InactivatedDateTime.HasValue)
        {
            return true;
        }

        user.InactivatedDateTime = DateTimeOffset.UtcNow;
        return true;
    }
}

