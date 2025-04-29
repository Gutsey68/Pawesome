namespace Pawesome.Models.DTOs;

public class UpdateUserDto
{
    public int Id { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public required string? Bio { get; set; }
    public required IFormFile? Photo { get; set; }
    public required string? ExistingPhoto { get; set; }
    public required string PhoneNumber { get; set; }
    //TODO [Tara] : ajouter menu déroulant addresses & de quoi modifier adresses 
}