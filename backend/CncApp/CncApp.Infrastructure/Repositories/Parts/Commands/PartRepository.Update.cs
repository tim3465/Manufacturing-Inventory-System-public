using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;
using CncApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CncApp.Infrastructure.Repositories;

public partial class PartRepository
{
    public async Task UpdateAsync(Part part, CancellationToken ct = default)
    {
        _context.Parts.Update(part);
        await Task.CompletedTask;
    }
}

