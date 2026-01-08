using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Parts;
using Moq;

namespace CncApp.Application.Tests.Services.Parts;

public partial class PartTests
{
    protected readonly Mock<IPartRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly PartService PartService;

    public PartTests()
    {
        MockRepository = new Mock<IPartRepository>();
        MockMapper = new Mock<IMapper>();
        PartService = new PartService(MockRepository.Object, MockMapper.Object);
    }
}

