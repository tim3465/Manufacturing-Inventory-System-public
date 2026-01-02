using CncApp.Application.Dtos.Users;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Users;

/// <summary>
/// Service for user provisioning operations.
/// </summary>
public partial class UserService
{
    private readonly IIdentityProvisioningService _identityProvisioningService;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public UserService(
        IIdentityProvisioningService identityProvisioningService,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _identityProvisioningService = identityProvisioningService;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }
}

