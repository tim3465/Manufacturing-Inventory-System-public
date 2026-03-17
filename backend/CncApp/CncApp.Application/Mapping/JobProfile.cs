using AutoMapper;
using CncApp.Application.Dtos.Jobs;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class JobProfile : Profile
{
    public JobProfile()
    {
        CreateMap<Job, JobDto>();

        CreateMap<Job, JobProductionDto>()
            .ForMember(dest => dest.StockLotId, opt => opt.MapFrom(src => src.StockLotId))
            .ForMember(dest => dest.LotNumber, opt => opt.MapFrom(src => src.StockLot != null ? src.StockLot.LotNumber : null))
            .ForMember(dest => dest.Shifts, opt => opt.Ignore())
            .ForMember(dest => dest.MachineName, opt => opt.Ignore())
            .ForMember(dest => dest.PartName, opt => opt.Ignore())
            .ForMember(dest => dest.PartNumber, opt => opt.Ignore())
            .ForMember(dest => dest.PartsCompleted, opt => opt.Ignore())
            .ForMember(dest => dest.PercentComplete, opt => opt.Ignore());

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateJobRequestDto, Job>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.StockLot, opt => opt.Ignore())
            .ForMember(dest => dest.Machine, opt => opt.Ignore())
            .ForMember(dest => dest.Shifts, opt => opt.Ignore());

        CreateMap<UpdateJobRequestDto, Job>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.StockLot, opt => opt.Ignore())
            .ForMember(dest => dest.Machine, opt => opt.Ignore())
            .ForMember(dest => dest.Shifts, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}

