using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Orders;

/// Validation mirrored from Infrastructure.Persistence.Configurations.OrderConfiguration where applicable.
public class OrderDto
{
    public int Id { get; set; }

    // TODO: Add properties based on Order entity
    // TODO: Add validation attributes matching Infrastructure configuration
}

