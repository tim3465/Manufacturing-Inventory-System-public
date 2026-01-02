using CncApp.Domain.Common;

namespace CncApp.Domain.Entities;

public class User : AuditableEntityBase
{
    public int IdentityUserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    // Email is NOT stored in Domain User - Identity owns email as source of truth
    // To get email, resolve via Identity using IdentityUserId

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
