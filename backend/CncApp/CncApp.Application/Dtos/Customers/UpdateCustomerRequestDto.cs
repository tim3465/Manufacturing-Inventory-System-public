using System.ComponentModel.DataAnnotations;

namespace CncApp.Application.Dtos.Customers;

public class UpdateCustomerRequestDto
{
    [Required(ErrorMessage = "CompanyName is required.")]
    [MaxLength(100, ErrorMessage = "CompanyName cannot exceed 100 characters.")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required.")]
    [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
    public string Address { get; set; } = string.Empty;
}
