
using CncApp.Domain.Common;
using CncApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CncApp.Domain.Entities;
public class UserRoleBase
{
    [Key]
    public int UserRoleId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public RoleType RoleType { get; set; }
}

public class UserRole : UserRoleBase
{
    public AuditTrail AuditTrail { get; set; } = new();

    public User User { get; set; } = null!;
}
