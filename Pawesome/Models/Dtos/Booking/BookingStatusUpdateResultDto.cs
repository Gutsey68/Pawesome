using Pawesome.Models.Dtos.Booking;

namespace Pawesome.Models.DTOs.Booking;

public class BookingStatusUpdateResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public BookingDto? Booking { get; set; }
}