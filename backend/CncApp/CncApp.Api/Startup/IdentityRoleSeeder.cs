using Microsoft.AspNetCore.Identity;

namespace CncApp.Api.Startup;

public static class IdentityRoleSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken ct = default)
    {
        var section = configuration.GetSection("RoleSeeding");
        var enabled = section.GetValue<bool>("Enabled");
        if (!enabled)
        {
            logger.LogInformation("Role seeding is disabled.");
            return;
        }

        var configuredRoles = section.GetSection("Roles").Get<string[]>() ?? Array.Empty<string>();
        var roles = configuredRoles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!roles.Any())
        {
            logger.LogInformation("Role seeding enabled but no roles configured.");
            return;
        }

        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        foreach (var roleName in roles)
        {
            ct.ThrowIfCancellationRequested();

            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole<int> { Name = roleName });
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            logger.LogInformation("Created identity role: {RoleName}", roleName);
        }
    }
}


