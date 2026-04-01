using CncApp.Application.Dtos.ShiftIssueLogs;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.ShiftIssueLogs;

public partial class ShiftIssueLogTests
{
    [Fact]
    public async Task GetByShiftAsync_WhenLogsExist_ReturnsMappedDtosWithDisplayNames()
    {
        // Arrange
        var shiftId = 1;
        var cancellationToken = CancellationToken.None;

        var logs = new List<ShiftIssueLog>
        {
            new ShiftIssueLog(shiftId, IssueTypeEnum.Setup, 5, "Scrap issue") { Id = 1, CreatedByUserId = 10 },
            new ShiftIssueLog(shiftId, IssueTypeEnum.Production, 0, "Downtime issue", TimeSpan.FromMinutes(30)) { Id = 2, CreatedByUserId = 20 }
        };

        var expectedDtos = new List<ShiftIssueLogResultDto>
        {
            new ShiftIssueLogResultDto { Id = 1, ShiftId = shiftId, IssueType = IssueTypeEnum.Setup, ScrapQuantity = 5, Description = "Scrap issue", CreatedByUserId = 10 },
            new ShiftIssueLogResultDto { Id = 2, ShiftId = shiftId, IssueType = IssueTypeEnum.Production, ScrapQuantity = 0, Description = "Downtime issue", Downtime = TimeSpan.FromMinutes(30), CreatedByUserId = 20 }
        };

        var users = new List<User>
        {
            new User { Id = 10, UserName = "jdoe", FirstName = "John", LastName = "Doe" },
            new User { Id = 20, UserName = "asmith", FirstName = "Alice", LastName = "Smith" }
        };

        MockRepository
            .Setup(r => r.ListByShiftAsync(shiftId, cancellationToken))
            .ReturnsAsync(logs);

        MockMapper
            .Setup(m => m.Map<List<ShiftIssueLogResultDto>>(logs))
            .Returns(expectedDtos);

        MockUserRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(users);

        // Act
        var result = await ShiftIssueLogService.GetByShiftAsync(shiftId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("John Doe", result[0].CreatedByUserDisplayName);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Alice Smith", result[1].CreatedByUserDisplayName);

        MockRepository.Verify(r => r.ListByShiftAsync(shiftId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<ShiftIssueLogResultDto>>(logs), Times.Once);
        MockUserRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByShiftAsync_WhenLogsExistWithNoFirstLastName_FallsBackToUserName()
    {
        // Arrange
        var shiftId = 1;
        var cancellationToken = CancellationToken.None;

        var logs = new List<ShiftIssueLog>
        {
            new ShiftIssueLog(shiftId, IssueTypeEnum.Setup, 5, "Scrap issue") { Id = 1, CreatedByUserId = 10 }
        };

        var expectedDtos = new List<ShiftIssueLogResultDto>
        {
            new ShiftIssueLogResultDto { Id = 1, ShiftId = shiftId, IssueType = IssueTypeEnum.Setup, ScrapQuantity = 5, Description = "Scrap issue", CreatedByUserId = 10 }
        };

        var users = new List<User>
        {
            new User { Id = 10, UserName = "jdoe", FirstName = null, LastName = null }
        };

        MockRepository
            .Setup(r => r.ListByShiftAsync(shiftId, cancellationToken))
            .ReturnsAsync(logs);

        MockMapper
            .Setup(m => m.Map<List<ShiftIssueLogResultDto>>(logs))
            .Returns(expectedDtos);

        MockUserRepository
            .Setup(r => r.ListAllAsync(cancellationToken))
            .ReturnsAsync(users);

        // Act
        var result = await ShiftIssueLogService.GetByShiftAsync(shiftId, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal("jdoe", result[0].CreatedByUserDisplayName);

        MockUserRepository.Verify(r => r.ListAllAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetByShiftAsync_WhenNoCreatedByUserId_DoesNotFetchUsers()
    {
        // Arrange
        var shiftId = 1;
        var cancellationToken = CancellationToken.None;

        var logs = new List<ShiftIssueLog>
        {
            new ShiftIssueLog(shiftId, IssueTypeEnum.Setup, 5, "Scrap issue") { Id = 1 }
        };

        var expectedDtos = new List<ShiftIssueLogResultDto>
        {
            new ShiftIssueLogResultDto { Id = 1, ShiftId = shiftId, IssueType = IssueTypeEnum.Setup, ScrapQuantity = 5, Description = "Scrap issue", CreatedByUserId = null }
        };

        MockRepository
            .Setup(r => r.ListByShiftAsync(shiftId, cancellationToken))
            .ReturnsAsync(logs);

        MockMapper
            .Setup(m => m.Map<List<ShiftIssueLogResultDto>>(logs))
            .Returns(expectedDtos);

        // Act
        var result = await ShiftIssueLogService.GetByShiftAsync(shiftId, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal(string.Empty, result[0].CreatedByUserDisplayName);

        MockRepository.Verify(r => r.ListByShiftAsync(shiftId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<ShiftIssueLogResultDto>>(logs), Times.Once);
        MockUserRepository.Verify(r => r.ListAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByShiftAsync_WhenNoLogsExist_ReturnsEmptyList()
    {
        // Arrange
        var shiftId = 999;
        var cancellationToken = CancellationToken.None;

        MockRepository
            .Setup(r => r.ListByShiftAsync(shiftId, cancellationToken))
            .ReturnsAsync(new List<ShiftIssueLog>());

        MockMapper
            .Setup(m => m.Map<List<ShiftIssueLogResultDto>>(It.IsAny<List<ShiftIssueLog>>()))
            .Returns(new List<ShiftIssueLogResultDto>());

        // Act
        var result = await ShiftIssueLogService.GetByShiftAsync(shiftId, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        MockRepository.Verify(r => r.ListByShiftAsync(shiftId, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<ShiftIssueLogResultDto>>(It.IsAny<List<ShiftIssueLog>>()), Times.Once);
        MockUserRepository.Verify(r => r.ListAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
