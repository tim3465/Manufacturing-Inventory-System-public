using CncApp.Application.Dtos.StockLots;
using CncApp.Domain.Entities;
using CncApp.Domain.Enums;
using Moq;
using Xunit;

namespace CncApp.Application.Tests.Services.StockLots;

public partial class StockLotTests
{
    [Fact]
    public async Task SearchActiveAsync_WhenResultsExist_ReturnsMappedPagedResult()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var request = new StockLotSearchRequestDto
        {
            Page = 1,
            PageSize = 25
        };

        var stockLots = new List<StockLot>
        {
            new StockLot("LOT-001", 1, 10, 25.5m, 1000.0m, StockLotConditionEnum.AsReceived, new DateTime(2025, 1, 1, 10, 0, 0))
            {
                Id = 1
            },
            new StockLot("LOT-002", 2, 20, 30.0m, 2000.0m, StockLotConditionEnum.Ground, new DateTime(2025, 1, 2, 10, 0, 0))
            {
                Id = 2
            }
        };

        var expectedDtos = new List<StockLotSummaryDto>
        {
            new StockLotSummaryDto
            {
                Id = 1,
                LotNumber = "LOT-001",
                AmountOfBars = 10,
                Diameter = 25.5m,
                BarLength = 1000.0m,
                Condition = StockLotConditionEnum.AsReceived,
                CheckedInDateTime = new DateTimeOffset(2025, 1, 1, 10, 0, 0, TimeSpan.Zero)
            },
            new StockLotSummaryDto
            {
                Id = 2,
                LotNumber = "LOT-002",
                AmountOfBars = 20,
                Diameter = 30.0m,
                BarLength = 2000.0m,
                Condition = StockLotConditionEnum.Ground,
                CheckedInDateTime = new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
            }
        };

        MockRepository
            .Setup(r => r.SearchActiveAsync(request, cancellationToken))
            .ReturnsAsync((stockLots, 2));

        MockMapper
            .Setup(m => m.Map<List<StockLotSummaryDto>>(stockLots))
            .Returns(expectedDtos);

        // Act
        var result = await StockLotService.SearchActiveAsync(request, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(25, result.PageSize);
        Assert.Equal("LOT-001", result.Items[0].LotNumber);
        Assert.Equal("LOT-002", result.Items[1].LotNumber);

        MockRepository.Verify(r => r.SearchActiveAsync(request, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotSummaryDto>>(stockLots), Times.Once);
    }

    [Fact]
    public async Task SearchActiveAsync_WhenNoResultsExist_ReturnsEmptyPagedResult()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var request = new StockLotSearchRequestDto
        {
            Page = 1,
            PageSize = 25
        };

        var stockLots = new List<StockLot>();
        var expectedDtos = new List<StockLotSummaryDto>();

        MockRepository
            .Setup(r => r.SearchActiveAsync(request, cancellationToken))
            .ReturnsAsync((stockLots, 0));

        MockMapper
            .Setup(m => m.Map<List<StockLotSummaryDto>>(stockLots))
            .Returns(expectedDtos);

        // Act
        var result = await StockLotService.SearchActiveAsync(request, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(25, result.PageSize);

        MockRepository.Verify(r => r.SearchActiveAsync(request, cancellationToken), Times.Once);
        MockMapper.Verify(m => m.Map<List<StockLotSummaryDto>>(stockLots), Times.Once);
    }

    [Fact]
    public async Task SearchActiveAsync_WhenFiltersApplied_PassesRequestToRepository()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var request = new StockLotSearchRequestDto
        {
            LotNumber = "LOT",
            CheckedInFrom = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            CheckedInTo = new DateTimeOffset(2025, 12, 31, 23, 59, 59, TimeSpan.Zero),
            DiameterExact = 25.5m,
            SortColumn = "LotNumber",
            SortDirection = "asc",
            Page = 2,
            PageSize = 10
        };

        var stockLots = new List<StockLot>();
        var expectedDtos = new List<StockLotSummaryDto>();

        MockRepository
            .Setup(r => r.SearchActiveAsync(request, cancellationToken))
            .ReturnsAsync((stockLots, 0));

        MockMapper
            .Setup(m => m.Map<List<StockLotSummaryDto>>(stockLots))
            .Returns(expectedDtos);

        // Act
        await StockLotService.SearchActiveAsync(request, cancellationToken);

        // Assert — verify the exact same request object was forwarded to the repository
        MockRepository.Verify(r => r.SearchActiveAsync(request, cancellationToken), Times.Once);
    }
}
