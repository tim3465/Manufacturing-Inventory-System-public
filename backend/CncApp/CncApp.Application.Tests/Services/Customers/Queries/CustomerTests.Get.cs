using CncApp.Application.Dtos.Customers;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Customers;

public partial class CustomerTests
{
    [Fact]
    public async Task GetAsync_WhenCustomerExists_ReturnsCustomerDto()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;

        var customer = new Customer("Acme Corp", "555-1234", "contact@acme.com", "123 Main St")
        {
            Id = customerId
        };

        var expectedDto = new CustomerDto
        {
            Id = customerId,
            CompanyName = "Acme Corp",
            Phone = "555-1234",
            Email = "contact@acme.com",
            Address = "123 Main St"
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(customerId, cancellationToken))
            .ReturnsAsync(customer);

        MockMapper
            .Setup(m => m.Map<CustomerDto>(customer))
            .Returns(expectedDto);

        // Act
        var result = await CustomerService.GetAsync(customerId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal("Acme Corp", result.CompanyName);

        MockRepository.Verify(r => r.GetByIdAsync(customerId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<CustomerDto>(customer), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var customerId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.GetByIdAsync(customerId, cancellationToken))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await CustomerService.GetAsync(customerId, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(customerId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<CustomerDto>(It.IsAny<Customer>()), Times.Never);
    }
}
