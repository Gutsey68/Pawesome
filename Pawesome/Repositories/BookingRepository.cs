using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using Pawesome.Models.Enums;

namespace Pawesome.Repositories
{
    /// <summary>
    /// Repository for managing booking operations in the database.
    /// Handles CRUD operations and custom queries for booking entities.
    /// </summary>
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookingRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for booking operations.</param>
        public BookingRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a booking by its ID with related details (advert, users, payments).
        /// </summary>
        /// <param name="bookingId">The ID of the booking to retrieve.</param>
        /// <returns>The booking with its related entities or null if not found.</returns>
        public async Task<Booking?> GetBookingByIdWithDetailsAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Advert)
                .ThenInclude(a => a.User)
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        /// <summary>
        /// Gets all bookings associated with a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="asBooker">
        /// If true, returns bookings where the user is the booker.
        /// If false, returns bookings where the user is the advertisement owner.
        /// </param>
        /// <returns>A list of bookings with related details.</returns>
        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId, bool asBooker)
        {
            return await _context.Bookings
                .Include(b => b.Advert)
                .ThenInclude(a => a.User)
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .Where(b => asBooker
                    ? b.BookerUserId == userId
                    : b.Advert.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all bookings for a specific advertisement.
        /// </summary>
        /// <param name="advertId">The ID of the advertisement.</param>
        /// <returns>A list of bookings for the specified advertisement.</returns>
        public async Task<List<Booking>> GetBookingsByAdvertIdAsync(int advertId)
        {
            return await _context.Bookings
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .Where(b => b.AdvertId == advertId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new booking in the database with initial status set to PendingConfirmation.
        /// </summary>
        /// <param name="booking">The booking entity to create.</param>
        /// <returns>The created booking with generated ID and timestamps.</returns>
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            booking.CreatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;
            booking.Status = BookingStatus.PendingConfirmation;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking;
        }

        /// <summary>
        /// Updates the status of a booking.
        /// Handles special behaviors for different status transitions.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="status">The new status to set.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        /// <remarks>
        /// If the status is set to InProgress and the start date matches today,
        /// the start time will be updated to the current time.
        /// If the status is set to Completed, the booking is automatically validated.
        /// </remarks>
        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
                return false;

            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;

            if (status == BookingStatus.InProgress)
            {
                if (booking.StartDate.Date == DateTime.UtcNow.Date)
                {
                    booking.StartDate = DateTime.UtcNow;
                }
            }
            else if (status == BookingStatus.Completed)
            {
                booking.IsValidated = true;
                booking.ValidatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Validates a booking, marking it as completed and setting validation flags.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to validate.</param>
        /// <returns>True if validation was successful, false otherwise.</returns>
        /// <remarks>
        /// This method can only be called on bookings with InProgress status.
        /// </remarks>
        public async Task<bool> ValidateBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
                return false;

            var allowedStatuses = new[]
            {
                BookingStatus.Accepted,
                BookingStatus.InProgress,
                BookingStatus.Completed
            };

            if (!allowedStatuses.Contains(booking.Status) || booking.IsValidated)
                return false;

            booking.Status = BookingStatus.Completed;
            booking.IsValidated = true;
            booking.ValidatedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Marks a booking as disputed with the specified reason.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to dispute.</param>
        /// <param name="reason">The reason for the dispute.</param>
        /// <returns>True if the dispute was successfully recorded, false otherwise.</returns>
        public async Task<bool> DisputeBookingAsync(int bookingId, string reason)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);

            if (booking == null)
                return false;

            booking.Status = BookingStatus.Disputed;
            booking.IsDisputed = true;
            booking.DisputeReason = reason;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Retrieves bookings that have been pending confirmation for too long.
        /// </summary>
        /// <param name="expirationDate">
        /// The cutoff date - bookings created before this date and still pending are considered expired.
        /// </param>
        /// <returns>A list of expired bookings with their related entities.</returns>
        public async Task<List<Booking>> GetExpiredBookingsAsync(DateTime expirationDate)
        {
            return await _context.Bookings
                .Include(b => b.Advert)
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .Where(b => b.Status == BookingStatus.PendingConfirmation &&
                            b.CreatedAt < expirationDate)
                .ToListAsync();
        }

        /// <summary>
        /// Gets bookings that should be automatically validated because the end date has passed.
        /// </summary>
        /// <param name="validationDeadline">
        /// The deadline - bookings that ended before this date and are not yet validated should be auto-validated.
        /// </param>
        /// <returns>A list of bookings that need to be automatically validated.</returns>
        /// <remarks>
        /// This is used for the automatic validation process that runs after a period of time
        /// if the user hasn't manually validated the booking.
        /// </remarks>
        public async Task<List<Booking>> GetBookingsToAutomaticallyValidateAsync(DateTime validationDeadline)
        {
            return await _context.Bookings
                .Include(b => b.Advert)
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .Where(b => b.Status == BookingStatus.InProgress &&
                            b.EndDate < validationDeadline &&
                            !b.IsValidated)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all pending bookings for adverts owned by the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the adverts.</param>
        /// <returns>A list of pending bookings for the user's adverts.</returns>
        public async Task<List<Booking>> GetPendingBookingsForUserAdvertsAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Advert)
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .Where(b => b.Advert.UserId == userId &&
                            b.Status == BookingStatus.PendingConfirmation)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
        
        /// <summary>
        /// Retrieves all active bookings (with status Accepted or InProgress).
        /// </summary>
        /// <returns>A list of active bookings including related advert, booker user, and payments.</returns>
        public async Task<List<Booking>> GetActiveBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.Advert)
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .Where(b => b.Status == BookingStatus.Accepted || b.Status == BookingStatus.InProgress)
                .ToListAsync();
        }
        
        /// <summary>
        /// Updates the status of an advert based on its associated bookings.
        /// </summary>
        /// <param name="advertId">The ID of the advert to update.</param>
        /// <returns>
        /// True if the advert status was updated successfully, false if the advert was not found.
        /// </returns>
        /// <remarks>
        /// - If there are active bookings (Accepted or InProgress), the advert is set to FullyBooked if all periods are covered, otherwise Active.
        /// - If there are no active bookings and the advert end date is in the past, the advert is set to Expired.
        /// - If the advert was previously FullyBooked but is no longer, it is set back to Active.
        /// </remarks>
        public async Task<bool> UpdateAdvertStatusBasedOnBookingsAsync(int advertId)
        {
            var advert = await _context.Adverts
                .Include(a => a.Bookings)
                .FirstOrDefaultAsync(a => a.Id == advertId);

            if (advert == null)
                return false;

            var bookings = advert.Bookings.Where(b =>
                b.Status == BookingStatus.Accepted ||
                b.Status == BookingStatus.InProgress).ToList();

            var now = DateTime.UtcNow;

            if (bookings.Any())
            {
                var isFullyBooked = IsAdvertFullyBooked(advert, bookings);

                if (isFullyBooked)
                {
                    advert.Status = AdvertStatus.FullyBooked;
                }
                else
                {
                    advert.Status = AdvertStatus.Active;
                }
            }
            else
            {
                if (advert.EndDate < now)
                {
                    advert.Status = AdvertStatus.Expired;
                }
                else if (advert.Status == AdvertStatus.FullyBooked)
                {
                    advert.Status = AdvertStatus.Active;
                }
            }

            advert.UpdatedAt = now;
            await _context.SaveChangesAsync();

            return true;
        }
        
        /// <summary>
        /// Determines if an advert is fully booked based on its bookings.
        /// An advert is considered fully booked if:
        /// - The earliest booking starts at or before the advert's start date.
        /// - The latest booking ends at or after the advert's end date.
        /// - All bookings are contiguous (no gaps between bookings).
        /// </summary>
        /// <param name="advert">The advert to check.</param>
        /// <param name="bookings">The list of bookings associated with the advert.</param>
        /// <returns>True if the advert is fully booked, otherwise false.</returns>
        private bool IsAdvertFullyBooked(Advert advert, List<Booking> bookings)
        {
            var sortedBookings = bookings
                .OrderBy(b => b.StartDate)
                .ToList();

            if (!sortedBookings.Any())
                return false;

            if (sortedBookings.First().StartDate > advert.StartDate)
                return false;

            if (sortedBookings.Last().EndDate < advert.EndDate)
                return false;

            for (int i = 0; i < sortedBookings.Count - 1; i++)
            {
                if (sortedBookings[i].EndDate < sortedBookings[i + 1].StartDate)
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Retrieves the first booking associated with a specific advert.
        /// </summary>
        /// <param name="advertId">The ID of the advert.</param>
        /// <returns>The first booking for the given advert, including related advert, booker user, and payments, or null if not found.</returns>
        public async Task<Booking?> GetBookingByAdvertIdAsync(int advertId)
        {
            return await _context.Bookings
                .Include(b => b.Advert)
                .Include(b => b.BookerUser)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.AdvertId == advertId);
        }
    }
}