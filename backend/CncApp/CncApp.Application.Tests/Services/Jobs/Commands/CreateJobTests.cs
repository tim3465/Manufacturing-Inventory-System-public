using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Jobs;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Jobs.Commands;

public class CreateJobTests
{
    private readonly Mock<IJobRepository> _mockRepository;
    private readonly Mock<AutoMapper.IMapper> _mockMapper;
    private readonly JobService _jobService;

    public CreateJobTests()
    {
        _mockRepository = new Mock<IJobRepository>();
        _mockMapper = new Mock<AutoMapper.IMapper>();
        _jobService = new JobService(_mockRepository.Object, _mockMapper.Object);
    }

    // TODO: Add test methods
}

