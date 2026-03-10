namespace CncApp.Application.Dtos.Customers;

public class CustomerDto
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}
