using AutoMapper;

using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Materials;

public partial class MaterialService
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMapper _mapper;

    public MaterialService(IMaterialRepository materialRepository, IMapper mapper)
    {
        _materialRepository = materialRepository;
        _mapper = mapper;
    }
}

