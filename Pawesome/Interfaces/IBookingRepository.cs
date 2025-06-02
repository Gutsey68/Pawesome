using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pawesome.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<Booking?> GetBookingByIdWithDetailsAsync(int bookingId);
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId, bool asBooker);
        Task<List<Booking>> GetBookingsByAdvertIdAsync(int advertId);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status);
        Task<bool> ValidateBookingAsync(int bookingId);
        Task<bool> DisputeBookingAsync(int bookingId, string reason);
        Task<List<Booking>> GetExpiredBookingsAsync(DateTime expirationDate);
        Task<List<Booking>> GetBookingsToAutomaticallyValidateAsync(DateTime validationDeadline);
        public Task<List<Booking>> GetPendingBookingsForUserAdvertsAsync(int userId);
        public Task<List<Booking>> GetActiveBookingsAsync();
        public Task<bool> UpdateAdvertStatusBasedOnBookingsAsync(int advertId);
        public Task<Booking?> GetBookingByAdvertIdAsync(int advertId);
    }
}