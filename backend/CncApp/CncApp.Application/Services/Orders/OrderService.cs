using AutoMapper;

using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Orders;

public partial class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }
}

