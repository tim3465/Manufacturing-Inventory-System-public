using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Materials;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Materials.Commands;

public class CreateMaterialTests
{
    private readonly Mock<IMaterialRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly MaterialService _materialService;

    public CreateMaterialTests()
    {
        _mockRepository = new Mock<IMaterialRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _materialService = new MaterialService(_mockRepository.Object, _mockMapper.Object);
    }

    // TODO: Add test methods
}

