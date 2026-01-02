using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class User : AuditableEntityBase
{
    public int IdentityUserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
