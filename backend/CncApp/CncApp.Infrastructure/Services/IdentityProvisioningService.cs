using CncApp.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CncApp.Infrastructure.Services;

/// <summary>
/// Implementation of IIdentityProvisioningService that uses ASP.NET Core Identity.
/// </summary>
public class IdentityProvisioningService : IIdentityProvisioningService
{
    private readonly UserManager<IdentityUser<int>> _userManager;

    public IdentityProvisioningService(UserManager<IdentityUser<int>> userManager)
    {
        _userManager = userManager;
    }

    /// <inheritdoc />
    public async Task<int> CreateIdentityUserAsync(string email, string userName, string password, CancellationToken ct = default)
    {
        // Identity UserName must equal Email (same value)
        var identityUser = new IdentityUser<int>
        {
            UserName = email, // UserName equals Email
            Email = email,
            EmailConfirmed = true // Skip email confirmation for admin-provisioned users
        };

        var result = await _userManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create Identity user: {errors}");
        }

        return identityUser.Id;
    }

    /// <inheritdoc />
    public async Task AssignRolesAsync(int identityUserId, IEnumerable<string> roles, CancellationToken ct = default)
    {
        var identityUser = await _userManager.FindByIdAsync(identityUserId.ToString());
        
        if (identityUser == null)
        {
            throw new InvalidOperationException($"Identity user with ID {identityUserId} not found.");
        }

        // Remove existing roles first to ensure clean state
        var existingRoles = await _userManager.GetRolesAsync(identityUser);
        if (existingRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(identityUser, existingRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to remove existing roles: {errors}");
            }
        }

        // Assign new roles
        var rolesList = roles.ToList();
        if (rolesList.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(identityUser, rolesList);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign roles: {errors}");
            }
        }
    }
}

