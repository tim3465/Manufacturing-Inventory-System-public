using CncApp.Application.Dtos.Customers;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Customers;

public partial class CustomerTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesCustomerAndReturnsId()
    {
        // Arrange
        var dto = new CreateCustomerRequestDto
        {
            CompanyName = "Acme Corp",
            Phone = "555-1234",
            Email = "contact@acme.com",
            Address = "123 Main St"
        };
        var cancellationToken = CancellationToken.None;

        var customer = new Customer(dto.CompanyName, dto.Phone, dto.Email, dto.Address)
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<Customer>(dto))
            .Returns(customer);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Customer>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await CustomerService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(1, result);

        MockMapper.Verify(m => m.Map<Customer>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(It.IsAny<Customer>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
