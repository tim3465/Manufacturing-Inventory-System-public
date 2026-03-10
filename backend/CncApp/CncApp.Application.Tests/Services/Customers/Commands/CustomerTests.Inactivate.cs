using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Customers;

public partial class CustomerTests
{
    [Fact]
    public async Task InactivateAsync_WhenCustomerExists_ReturnsTrue()
    {
        // Arrange
        var customerId = 1;
        var inactivatedByUserId = 5;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(customerId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(true);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await CustomerService.InactivateAsync(customerId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.True(result);

        MockRepository.Verify(r => r.InactivateAsync(customerId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task InactivateAsync_WhenCustomerDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var customerId = 999;
        int? inactivatedByUserId = null;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.InactivateAsync(customerId, inactivatedByUserId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await CustomerService.InactivateAsync(customerId, inactivatedByUserId, cancellationToken);

        // Assert
        Assert.False(result);

        MockRepository.Verify(r => r.InactivateAsync(customerId, inactivatedByUserId, cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Never);
    }
}
