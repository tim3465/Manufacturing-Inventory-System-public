using AutoMapper;
using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class JobProfile : Profile
{
    public JobProfile()
    {
        CreateMap<Job, JobDto>();

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateJobRequestDto, Job>()
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.StockLot, opt => opt.Ignore())
            .ForMember(dest => dest.Machine, opt => opt.Ignore())
            .ForMember(dest => dest.Shifts, opt => opt.Ignore());
    }
}

