using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Jobs;

/// Validation mirrored from Infrastructure.Persistence.Configurations.JobConfiguration where applicable.
public class JobDto
{
    public int Id { get; set; }

    // TODO: Add properties based on Job entity
    // TODO: Add validation attributes matching Infrastructure configuration
}

