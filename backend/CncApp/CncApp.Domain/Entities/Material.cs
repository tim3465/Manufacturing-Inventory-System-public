using System.ComponentModel.DataAnnotations;
namespace CncApp.Domain.Entities;
public class MaterialBase
{
    [Key]
    public int MaterialId { get; set; }

    [Required, MaxLength(100)]
    public string HeatNumber { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string MaterialName { get; set; } = string.Empty; // e.g., 17-4, M2
    }
public class Material : MaterialBase
{
    public ICollection<StockLot> StockLots { get; set; } = new List<StockLot>();
}