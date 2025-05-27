using Pawesome.Models.enums;

namespace Pawesome.Models.Dtos.Booking;

public class UpdateBookingDto
{
    public int Id { get; set; }
    public BookingStatus Status { get; set; }
    public bool? IsValidated { get; set; }
    public bool? IsDisputed { get; set; }
    public string? DisputeReason { get; set; }
}