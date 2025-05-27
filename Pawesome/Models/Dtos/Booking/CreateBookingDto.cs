using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.DTOs.Booking;

public class CreateBookingDto
{
    public int AdvertId { get; set; }
        
    public DateTime StartDate { get; set; }
        
    public DateTime EndDate { get; set; }
        
    [MaxLength(500)]
    public string? Message { get; set; }
}