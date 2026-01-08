using AutoMapper;
using CncApp.Application.Dtos.Parts;
using CncApp.Application.Interfaces.Repositories;
using CncApp.Domain.Entities;

namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    public async Task<int> CreateAsync(CreatePartRequestDto dto, CancellationToken ct = default)
    {
        var part = _mapper.Map<Part>(dto);

        await _partRepository.AddAsync(part, ct);
        await _partRepository.SaveChangesAsync(ct);

        return part.Id;
    }
}

