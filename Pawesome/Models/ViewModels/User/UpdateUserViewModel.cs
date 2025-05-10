namespace Pawesome.Models.ViewModels.User;

public class UpdateUserViewModel
{
    public int Id { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public required string? Bio { get; set; }
    public required IFormFile? Photo { get; set; }
    public required string? ExistingPhoto { get; set; }
    public required string? PhoneNumber { get; set; }

    public string? StreetAddress { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "France"; 
}