
namespace CncApp.Domain.Common;

public class AuditTrail
{
    public DateTime CreatedAtDateTime { get; set; }
    public int? CreatedByUserId { get; set; }

    public DateTime? UpdatedAtDateTime { get; set; }
    public int? UpdatedByUserId { get; set; }

    public DateTime? DisabledAtDateTime { get; set; }
    public int? DisabledByUserId { get; set; }
}
