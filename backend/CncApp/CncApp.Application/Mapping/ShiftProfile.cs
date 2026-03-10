using AutoMapper;
using CncApp.Application.Dtos.Shifts;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class ShiftProfile : Profile
{
    public ShiftProfile()
    {
        CreateMap<Shift, ShiftDto>()
            .ForMember(dest => dest.OperatorName,
                opt => opt.MapFrom(src =>
                    src.Operator != null
                        ? (src.Operator.FirstName != null && src.Operator.LastName != null
                            ? $"{src.Operator.FirstName} {src.Operator.LastName}"
                            : src.Operator.UserName)
                        : string.Empty))
            .ForMember(dest => dest.PartName,
                opt => opt.MapFrom(src =>
                    src.Job != null && src.Job.Order != null && src.Job.Order.Part != null
                        ? src.Job.Order.Part.PartName
                        : string.Empty))
            .ForMember(dest => dest.PartNumber,
                opt => opt.MapFrom(src =>
                    src.Job != null && src.Job.Order != null && src.Job.Order.Part != null
                        ? src.Job.Order.Part.PartNumber
                        : string.Empty));

        CreateMap<Shift, ShiftResultDto>();
        CreateMap<CreateShiftRequestDto, Shift>();
    }
}

