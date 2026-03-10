using AutoMapper;
using CncApp.Application.Dtos.Orders;
using CncApp.Domain.Entities;

namespace CncApp.Application.Mapping;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CompanyName : string.Empty));

        // Create DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<CreateOrderRequestDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Part, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Jobs, opt => opt.Ignore());

        // Update DTO maps only client-provided fields; audit/identity fields are server-controlled.
        CreateMap<UpdateOrderRequestDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Part, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Jobs, opt => opt.Ignore());
    }
}

