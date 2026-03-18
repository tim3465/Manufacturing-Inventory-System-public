using CncApp.Application.Dtos.Customers;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Customers;

public partial class CustomerTests
{
    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_UpdatesPropertiesAndReturnsDto()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;

        var dto = new UpdateCustomerRequestDto
        {
            CompanyName = "Updated Corp",
            Phone = "555-9999",
            Email = "new@updated.com",
            Address = "456 Oak Ave"
        };

        var customer = new Customer("Acme Corp", "555-1234", "old@acme.com", "123 Main St")
        {
            Id = customerId
        };

        var expectedDto = new CustomerDto
        {
            Id = customerId,
            CompanyName = dto.CompanyName,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(customerId, cancellationToken))
            .ReturnsAsync(customer);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        MockMapper
            .Setup(m => m.Map<CustomerDto>(customer))
            .Returns(expectedDto);

        // Act
        var result = await CustomerService.UpdateAsync(customerId, dto, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.CompanyName, result.CompanyName);
        Assert.Equal(dto.Phone, result.Phone);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.Address, result.Address);

        MockRepository.Verify(r => r.GetByIdAsync(customerId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<CustomerDto>(customer), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var customerId = 999;
        var cancellationToken = CancellationToken.None;

        var dto = new UpdateCustomerRequestDto
        {
            CompanyName = "Updated Corp",
            Phone = "555-9999",
            Email = "new@updated.com",
            Address = "456 Oak Ave"
        };

        MockRepository
            .Setup(r => r.GetByIdAsync(customerId, cancellationToken))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await CustomerService.UpdateAsync(customerId, dto, cancellationToken);

        // Assert
        Assert.Null(result);

        MockRepository.Verify(r => r.GetByIdAsync(customerId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
        MockMapper.Verify(m => m.Map<CustomerDto>(It.IsAny<Customer>()), Times.Never);
    }
}
