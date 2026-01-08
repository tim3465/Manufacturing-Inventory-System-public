using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Shifts;
using Moq;

namespace CncApp.Application.Tests.Services.Shifts;

public partial class ShiftTests
{
    protected readonly Mock<IShiftRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly ShiftService ShiftService;

    public ShiftTests()
    {
        MockRepository = new Mock<IShiftRepository>();
        MockMapper = new Mock<IMapper>();
        ShiftService = new ShiftService(MockRepository.Object, MockMapper.Object);
    }
}

