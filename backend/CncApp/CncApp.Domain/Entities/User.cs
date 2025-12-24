using System.ComponentModel.DataAnnotations;
using CncApp.Domain.Common;
namespace CncApp.Domain.Entities;
public class UserBase
{
    [Key]
    public int UserId { get; set; }

    [Required, MaxLength(200)]
    public string UserName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? FirstName { get; set; }

    [MaxLength(200)]
    public string? LastName { get; set; }

    [MaxLength(320)]
    public string? Email { get; set; }

}

public class User: UserBase
{
    public AuditTrail AuditTrail { get; set; } = new();

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();

}