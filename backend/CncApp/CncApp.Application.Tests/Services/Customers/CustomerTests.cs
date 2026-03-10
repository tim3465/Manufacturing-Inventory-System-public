using AutoMapper;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Application.Services.Customers;
using Moq;

namespace CncApp.Application.Tests.Services.Customers;

public partial class CustomerTests
{
    protected readonly Mock<ICustomerRepository> MockRepository;
    protected readonly Mock<IMapper> MockMapper;
    protected readonly CustomerService CustomerService;

    public CustomerTests()
    {
        MockRepository = new Mock<ICustomerRepository>();
        MockMapper = new Mock<IMapper>();
        CustomerService = new CustomerService(MockRepository.Object, MockMapper.Object);
    }
}
