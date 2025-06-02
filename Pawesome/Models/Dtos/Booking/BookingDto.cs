using Pawesome.Models.Dtos.Payment;
using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using Pawesome.Models.Enums;

namespace Pawesome.Models.Dtos.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public BookingStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsValidated { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public bool IsDisputed { get; set; }
    public string? DisputeReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
        
    public int AdvertId { get; set; }
    public string AdvertTitle { get; set; } = string.Empty;
    public AdvertStatus AdvertStatus { get; set; }
        
    public int BookerUserId { get; set; }
    public string BookerUserName { get; set; } = string.Empty;
    public string BookerUserEmail { get; set; } = string.Empty;
    public string? PetSitterPhoto { get; set; }
    public string? BookerPhoto { get; set; }
    public string? AdditionalInformation { get; set; }
        
    public int PetSitterUserId { get; set; }
    public string PetSitterUserName { get; set; } = string.Empty;
        
    public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    
    public string StatusLabel => Status switch
    {
        BookingStatus.PendingConfirmation => "En attente de confirmation",
        BookingStatus.Accepted => "Acceptée",
        BookingStatus.InProgress => "En cours",
        BookingStatus.Completed => "Terminée",
        BookingStatus.Validated => "Validée",
        BookingStatus.Declined => "Refusée",
        BookingStatus.Cancelled => "Annulée",
        BookingStatus.Disputed => "Litigée",
        _ => Status.ToString()
    };
}