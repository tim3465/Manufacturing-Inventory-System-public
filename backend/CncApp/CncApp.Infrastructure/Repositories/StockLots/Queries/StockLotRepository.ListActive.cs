using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository : IStockLotRepository
{
    public async Task<List<StockLot>> ListActiveAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

