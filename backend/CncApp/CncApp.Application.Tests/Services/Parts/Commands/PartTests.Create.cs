using AutoMapper;
using CncApp.Application.Dtos.Parts;
using CncApp.Domain.Entities;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.Parts;

public partial class PartTests
{
    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesPartAndReturnsId()
    {
        // Arrange
        var dto = new CreatePartRequestDto
        {
            PartName = "Test Part",
            PartNumber = "TP-001",
            ApproxPartCycleTime = TimeSpan.FromMinutes(5),
            CheckPerPart = 10
        };
        var cancellationToken = CancellationToken.None;

        var part = new Part(dto.PartName, dto.PartNumber, dto.ApproxPartCycleTime, dto.CheckPerPart)
        {
            Id = 1
        };

        MockMapper
            .Setup(m => m.Map<Part>(dto))
            .Returns(part);

        MockRepository
            .Setup(r => r.AddAsync(It.IsAny<Part>(), cancellationToken))
            .Returns(Task.CompletedTask);

        MockRepository
            .Setup(r => r.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await PartService.CreateAsync(dto, cancellationToken);

        // Assert
        Assert.Equal(1, result);

        MockMapper.Verify(m => m.Map<Part>(dto), Times.Once);
        MockRepository.Verify(r => r.AddAsync(It.IsAny<Part>(), cancellationToken), Times.Once);
        MockRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}

