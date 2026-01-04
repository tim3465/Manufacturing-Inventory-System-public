using CncApp.Application.Interfaces.Repositories;
using CncApp.Infrastructure.Persistence;

namespace CncApp.Infrastructure.Repositories;

public partial class StockLotRepository : IStockLotRepository
{
    private readonly AppDbContext _context;

    public StockLotRepository(AppDbContext context)
    {
        _context = context;
    }
}
