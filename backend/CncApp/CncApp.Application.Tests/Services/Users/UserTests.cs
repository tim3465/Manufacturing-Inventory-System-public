using AutoMapper;
using CncApp.Application.Interfaces;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Users;
using Moq;

namespace CncApp.Application.Tests.Services.Users;

public partial class UserTests
{
    protected readonly Mock<IUserRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly Mock<IIdentityProvisioningService> MockIdentityProvisioningService;
    protected readonly Mock<ICurrentUserService> MockCurrentUserService;
    protected readonly UserService UserService;

    public UserTests()
    {
        MockRepository = new Mock<IUserRepository>();
        MockMapper = new Mock<IMapper>();
        MockIdentityProvisioningService = new Mock<IIdentityProvisioningService>();
        MockCurrentUserService = new Mock<ICurrentUserService>();

        UserService = new UserService(
            MockIdentityProvisioningService.Object,
            MockRepository.Object,
            MockCurrentUserService.Object,
            MockMapper.Object);
    }
}

