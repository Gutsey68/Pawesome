using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Pawesome.Models.DTOs;

public class RegisterDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}