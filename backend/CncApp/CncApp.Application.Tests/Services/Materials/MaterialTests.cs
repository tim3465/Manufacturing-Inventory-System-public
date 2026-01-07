using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Materials;
using Moq;

namespace CncApp.Application.Tests.Services.Materials;

public partial class MaterialTests
{
    protected readonly Mock<IMaterialRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly MaterialService MaterialService;

    public MaterialTests()
    {
        MockRepository = new Mock<IMaterialRepository>();
        MockMapper = new Mock<IMapper>();
        MaterialService = new MaterialService(MockRepository.Object, MockMapper.Object);
    }
}

