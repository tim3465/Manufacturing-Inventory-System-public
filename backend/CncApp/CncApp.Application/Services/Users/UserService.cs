using AutoMapper;
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
    public readonly IIdentityProvisioningService _identityProvisioningService;
    public readonly IUserRepository _userRepository;
    public readonly ICurrentUserService _currentUserService;
    public readonly IMapper _mapper;

    public UserService(
        IIdentityProvisioningService identityProvisioningService,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _identityProvisioningService = identityProvisioningService;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }
}

