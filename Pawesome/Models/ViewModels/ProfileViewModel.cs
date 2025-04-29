namespace Pawesome.Models.ViewModels;

public class ProfileViewModel
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Bio { get; set; }
    public required string Status { get; set; }
    public required bool IsVerified { get; set; }
    public required decimal BalanceAccount { get; set; }
    public float? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    
    public List<PetViewModel> Pets { get; set; } = new();
    
}