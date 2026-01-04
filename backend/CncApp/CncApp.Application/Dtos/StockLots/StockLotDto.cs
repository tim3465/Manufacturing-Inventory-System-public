using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.StockLots;

/// Validation mirrored from Infrastructure.Persistence.Configurations.StockLotConfiguration where applicable.
public class StockLotDto
{
    public int Id { get; set; }

    // TODO: Add properties based on StockLot entity
    // TODO: Add validation attributes matching Infrastructure configuration
}

