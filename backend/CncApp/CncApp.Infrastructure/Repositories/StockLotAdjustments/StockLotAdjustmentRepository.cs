using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotAdjustmentRepository : IStockLotAdjustmentRepository
{
    private readonly AppDbContext _context;

    public StockLotAdjustmentRepository(AppDbContext context)
    {
        _context = context;
    }
}

