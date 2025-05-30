using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Dtos.Booking;
using Pawesome.Models.ViewModels.Advert;
using Pawesome.Models.ViewModels.Pet;

namespace Pawesome.Models.ViewModels.User;

public class ProfileViewModel
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Bio { get; set; }
    public required string Photo { get; set; }
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
    
    public List<PetSittingAdvertDto> Adverts { get; set; } = new();
    public List<BookingDto> PendingBookings { get; set; } = new List<BookingDto>();
    
}