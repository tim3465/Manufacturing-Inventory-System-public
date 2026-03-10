using AutoMapper;
using CncApp.Application.Dtos.Customers;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerDto>();

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateCustomerRequestDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore());
    }
}
