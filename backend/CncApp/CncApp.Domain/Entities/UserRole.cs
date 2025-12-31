using CncApp.Domain.Common;
using CncApp.Domain.Enums;

namespace CncApp.Domain.Entities;

public class UserRole : AuditableEntityBase
{
    public int UserId { get; set; }

    public RoleType RoleType { get; set; }

    public User User { get; set; } = null!;
}
