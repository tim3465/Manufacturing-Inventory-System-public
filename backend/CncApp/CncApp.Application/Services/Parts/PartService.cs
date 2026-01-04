using AutoMapper;
using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Parts;

public partial class PartService
{
    private readonly IPartRepository _partRepository;
    private readonly IMapper _mapper;

    public PartService(IPartRepository partRepository, IMapper mapper)
    {
        _partRepository = partRepository;
        _mapper = mapper;
    }
}

