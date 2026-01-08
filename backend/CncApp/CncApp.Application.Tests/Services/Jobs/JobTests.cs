using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Jobs;
using Moq;

namespace CncApp.Application.Tests.Services.Jobs;

public partial class JobTests
{
    protected readonly Mock<IJobRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly JobService JobService;

    public JobTests()
    {
        MockRepository = new Mock<IJobRepository>();
        MockMapper = new Mock<IMapper>();
        JobService = new JobService(MockRepository.Object, MockMapper.Object);
    }
}

