namespace Pawesome.Models.ViewModels.Auth;

public class ResetPasswordViewModel
{
    public required string Email { get; set; }
    public required string Code { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}