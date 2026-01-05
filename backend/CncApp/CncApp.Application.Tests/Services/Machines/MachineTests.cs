using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Machines;
using Moq;

namespace CncApp.Application.Tests.Services.Machines;

public partial class MachineTests
{
    protected readonly Mock<IMachineRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly MachineService MachineService;

    public MachineTests()
    {
        MockRepository = new Mock<IMachineRepository>();
        MockMapper = new Mock<IMapper>();
        MachineService = new MachineService(MockRepository.Object, MockMapper.Object);
    }
}

