using System.ComponentModel.DataAnnotations;

namespace CncApp.Api.ApiDtos;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}

jhbnklgkjhkjhjkh;
    ;kljlkj
    kjlkj
    jl;kj
    lkj
    jkkjl
    jkl
    kj
    lkjl
    kjl
    kjkjljkl
    jkl
    kjl
    jkljkjkkljlklkj
    lklk
    ljlk