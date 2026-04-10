using AutoMapper;
using CncApp.Application.Dtos.ShiftIssueLogs;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class ShiftIssueLogProfile : Profile
{
    public ShiftIssueLogProfile()
    {
        CreateMap<ShiftIssueLog, ShiftIssueLogResultDto>()
            .ForMember(dest => dest.CreatedByUserDisplayName, opt => opt.Ignore());

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateShiftIssueLogRequestDto, ShiftIssueLog>()
            .ForMember(dest => dest.Shift, opt => opt.Ignore());
    }
}
