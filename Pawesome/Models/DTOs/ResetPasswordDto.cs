namespace Pawesome.Models.DTOs;

public class ResetPasswordDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public required string Code { get; set; }
}