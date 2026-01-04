using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Parts;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Parts.Commands;

public class CreatePartTests
{
    private readonly Mock<IPartRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly PartService _partService;

    public CreatePartTests()
    {
        _mockRepository = new Mock<IPartRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _partService = new PartService(_mockRepository.Object, _mockMapper.Object);
    }

    // TODO: Add test methods
}

