using Pawesome.Models.Dtos.Booking;
using Pawesome.Models.DTOs.Booking;
using Pawesome.Models.enums;

namespace Pawesome.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDto?> GetBookingByIdAsync(int bookingId);
        Task<List<BookingDto>> GetUserBookingsAsync(int userId, bool asBooker);
        Task<BookingDto> CreateBookingAsync(CreateBookingDto createDto, int userId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<bool> ValidateBookingAsync(int bookingId);
        Task<bool> DisputeBookingAsync(int bookingId, string reason);
        Task<bool> ProcessAutomaticValidationsAsync();
        Task<bool> ProcessExpiredBookingsAsync();
        public Task<List<BookingDto>> GetPendingBookingsForUserAdvertsAsync(int userId);
        public int GetBookingsCount();
        public List<BookingDto> GetAllBookings();
    }
}