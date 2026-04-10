using CncApp.Application.Dtos.Customers;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Customers;

public partial class CustomerTests
{
    [Fact]
    public async Task ListAllAsync_WhenCustomersExist_ReturnsListOfCustomerDtos()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var customers = new List<Customer>
        {
            new Customer("Acme Corp", "555-1234", "acme@acme.com", "123 Main St") { Id = 1 },
            new Customer("Beta Inc", "555-5678", "beta@beta.com", "456 Oak Ave") { Id = 2 }
        };

        var expectedDtos = new List<CustomerDto>
        {
            new CustomerDto { Id = 1, CompanyName = "Acme Corp", Phone = "555-1234", Email = "acme@acme.com", Address = "123 Main St" },
            new CustomerDto { Id = 2, CompanyName = "Beta Inc", Phone = "555-5678", Email = "beta@beta.com", Address = "456 Oak Ave" }
        };

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(customers);

        MockMapper
            .Setup(m => m.Map<List<CustomerDto>>(customers))
            .Returns(expectedDtos);

        // Act
        var result = await CustomerService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<CustomerDto>>(customers), Times.Once);
    }

    [Fact]
    public async Task ListAllAsync_WhenNoCustomersExist_ReturnsEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var customers = new List<Customer>();

        MockRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(customers);

        MockMapper
            .Setup(m => m.Map<List<CustomerDto>>(customers))
            .Returns(new List<CustomerDto>());

        // Act
        var result = await CustomerService.ListAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<CustomerDto>>(customers), Times.Once);
    }
}
