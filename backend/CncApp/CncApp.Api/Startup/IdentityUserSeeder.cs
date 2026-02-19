using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CncApp.Api.Startup;

public static class IdentityUserSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger logger,
        CancellationToken ct = default)
    {
        var section = configuration.GetSection("UserSeeding");
        var enabled = section.GetValue<bool>("Enabled");
        if (!enabled)
        {
            logger.LogInformation("User seeding is disabled.");
            return;
        }

        var overwriteRoles = section.GetValue<bool>("OverwriteRoles");
        var overwritePassword = section.GetValue<bool>("OverwritePassword");
        var configuredUsers = section.GetSection("Users").Get<SeedUserConfig[]>() ?? Array.Empty<SeedUserConfig>();

        var users = configuredUsers
            .Where(u => !string.IsNullOrWhiteSpace(u.Email))
            .Select(u => u with { Email = u.Email.Trim() })
            .ToList();

        if (!users.Any())
        {
            logger.LogInformation("User seeding enabled but no users configured.");
            return;
        }

        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<int>>>();
        var identityProvisioning = scope.ServiceProvider.GetRequiredService<IIdentityProvisioningService>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        foreach (var user in users)
        {
            ct.ThrowIfCancellationRequested();

            var existingIdentityUser = await userManager.FindByEmailAsync(user.Email);
            if (existingIdentityUser == null)
            {
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    logger.LogWarning(
                        "Seed user skipped because Password is missing for Email '{Email}'. (Put passwords in appsettings.Local.json.)",
                        user.Email);
                    continue;
                }

                // Validate roles exist BEFORE creating the Identity user to avoid orphaned records.
                if (user.Roles.Any())
                {
                    await identityProvisioning.ValidateRolesExistAsync(user.Roles);
                }

                var identityUserId = await identityProvisioning.CreateIdentityUserAsync(
                    user.Email,
                    user.Email, // UserName equals Email
                    user.Password,
                    ct);

                if (user.Roles.Any())
                {
                    await identityProvisioning.AssignRolesAsync(identityUserId, user.Roles, ct);
                }

                var domainUser = new User
                {
                    IdentityUserId = identityUserId,
                    UserName = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                await userRepository.AddAsync(domainUser, ct);
                await userRepository.SaveChangesAsync(ct);

                logger.LogInformation(
                    "Seeded user: {Email} (IdentityId: {IdentityUserId}, DomainId: {DomainUserId})",
                    user.Email,
                    identityUserId,
                    domainUser.Id);

                continue;
            }

            // Existing Identity user: optionally converge roles + ensure Domain user exists.
            if (overwritePassword)
            {
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    logger.LogWarning(
                        "Seed user password overwrite requested but Password is missing for Email '{Email}'.",
                        user.Email);
                }
                else
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(existingIdentityUser);
                    var resetResult = await userManager.ResetPasswordAsync(existingIdentityUser, token, user.Password);
                    if (!resetResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to reset password for seed user '{user.Email}': {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");
                    }

                    logger.LogInformation("Reset password for existing seed user: {Email}", user.Email);
                }
            }

            if (overwriteRoles && user.Roles.Any())
            {
                await identityProvisioning.ValidateRolesExistAsync(user.Roles);
                await identityProvisioning.AssignRolesAsync(existingIdentityUser.Id, user.Roles, ct);
                logger.LogInformation("Updated roles for existing seed user: {Email}", user.Email);
            }

            var existingDomainUser = await userRepository.GetByIdentityUserIdAsync(existingIdentityUser.Id, ct);
            if (existingDomainUser == null)
            {
                var domainUser = new User
                {
                    IdentityUserId = existingIdentityUser.Id,
                    UserName = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                await userRepository.AddAsync(domainUser, ct);
                await userRepository.SaveChangesAsync(ct);

                logger.LogInformation(
                    "Created missing Domain user for existing Identity user: {Email} (IdentityId: {IdentityUserId}, DomainId: {DomainUserId})",
                    user.Email,
                    existingIdentityUser.Id,
                    domainUser.Id);
            }
            else
            {
                // Optional: keep domain name fields in sync when provided in config.
                var changed = false;

                if (!string.IsNullOrWhiteSpace(user.FirstName) && existingDomainUser.FirstName != user.FirstName)
                {
                    existingDomainUser.FirstName = user.FirstName;
                    changed = true;
                }

                if (!string.IsNullOrWhiteSpace(user.LastName) && existingDomainUser.LastName != user.LastName)
                {
                    existingDomainUser.LastName = user.LastName;
                    changed = true;
                }

                if (changed)
                {
                    await userRepository.SaveChangesAsync(ct);
                    logger.LogInformation("Updated Domain user profile for seed user: {Email}", user.Email);
                }
            }
        }
    }

    private sealed record SeedUserConfig
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public List<string> Roles { get; init; } = new();
    }
}


