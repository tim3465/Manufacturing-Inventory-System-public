using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Shifts;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Shifts.Commands;

public class CreateShiftTests
{
    private readonly Mock<IShiftRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly ShiftService _shiftService;

    public CreateShiftTests()
    {
        _mockRepository = new Mock<IShiftRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _shiftService = new ShiftService(_mockRepository.Object, _mockMapper.Object);
    }

    // TODO: Add test methods
}

