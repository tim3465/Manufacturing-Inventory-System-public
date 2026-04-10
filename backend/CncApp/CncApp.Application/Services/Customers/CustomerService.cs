using AutoMapper;
using CncApp.Application.Interfaces.Repositories;

namespace CncApp.Application.Services.Customers;

public partial class CustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }
}
