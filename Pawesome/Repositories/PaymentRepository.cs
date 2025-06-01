using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using Pawesome.Models.Enums;

namespace Pawesome.Repositories;

/// <summary>
/// Repository responsible for handling payment data operations
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRepository"/> class
    /// </summary>
    /// <param name="context">The application database context</param>
    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new payment record in the database
    /// </summary>
    /// <param name="payment">The payment entity to create</param>
    /// <returns>The created payment entity</returns>
    public async Task<Payment> CreatePaymentAsync(Payment payment)
    {
        var existingPayment = await _context.Payments
            .FirstOrDefaultAsync(p => p.SessionId == payment.SessionId);
        
        if (existingPayment != null)
        {
            return existingPayment;
        }
    
        _context.Payments.Add(payment);
    
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            var conflictingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.SessionId == payment.SessionId);
            
            if (conflictingPayment != null)
            {
                return conflictingPayment;
            }
        
            throw;
        }
    
        return payment;
    }

    /// <summary>
    /// Retrieves a payment by its Stripe session ID
    /// </summary>
    /// <param name="sessionId">The Stripe session ID to search for</param>
    /// <returns>The payment entity if found, null otherwise</returns>
    public async Task<Payment?> GetPaymentBySessionIdAsync(string sessionId)
    {
        return await _context.Payments.FirstOrDefaultAsync(p => p.SessionId == sessionId);
    }

    /// <summary>
    /// Retrieves all payments for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose payments to retrieve</param>
    /// <returns>A list of payment entities for the specified user</returns>
    public async Task<List<Payment>> GetUserPaymentsAsync(int userId)
    {
        return await _context.Payments
            .Include(p => p.Booking)
            .ThenInclude(b => b.Advert)
            .ThenInclude(a => a.User)
            .Include(p => p.Booking)
            .ThenInclude(b => b.BookerUser)
            .Where(p => p.Booking.BookerUserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Updates the status of a payment identified by its session ID
    /// </summary>
    /// <param name="sessionId">The session ID of the payment to update</param>
    /// <param name="status">The new payment status</param>
    /// <param name="paymentIntentId">Optional payment intent ID</param>
    /// <returns>The updated payment entity if found, null otherwise</returns>
    public async Task<Payment?> UpdatePaymentStatusAsync(string sessionId, PaymentStatus status, string? paymentIntentId = null)
    {
        var payment = await _context.Payments
            .Include(p => p.Booking)
            .FirstOrDefaultAsync(p => p.SessionId == sessionId);

        if (payment == null)
            return null;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;
    
        if (paymentIntentId != null)
        {
            payment.PaymentIntentId = paymentIntentId;
        }

        if (status == PaymentStatus.Captured)
        {
            payment.Booking.Status = BookingStatus.Accepted;
            payment.Booking.UpdatedAt = DateTime.UtcNow;
        }
        else if (status == PaymentStatus.Failed)
        {
            payment.Booking.Status = BookingStatus.Declined;
            payment.Booking.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return payment;
    }
    
    /// <summary>
    /// Retrieves all payments made by a specific user for a specific advert.
    /// </summary>
    /// <param name="userId">The ID of the user who made the payments.</param>
    /// <param name="advertId">The ID of the advert associated with the payments.</param>
    /// <returns>A list of payment entities matching the user and advert.</returns>
    public async Task<List<Payment>> GetPaymentsByUserAndAdvertAsync(int userId, int advertId)
    {
        return await _context.Payments
            .Include(p => p.Booking)
            .ThenInclude(b => b.Advert)
            .Where(p => p.Booking.BookerUserId == userId && p.Booking.AdvertId == advertId)
            .ToListAsync();
    }
    
    /// <summary>
    /// Retrieves a payment by its ID, including related booking, advert, and user details.
    /// </summary>
    /// <param name="paymentId">The ID of the payment to retrieve.</param>
    /// <returns>The payment entity with related details if found, otherwise null.</returns>
    public async Task<Payment?> GetByIdWithDetailsAsync(int paymentId)
    {
        return await _context.Payments
            .Include(p => p.Booking)
            .ThenInclude(b => b.Advert)
            .ThenInclude(a => a.User)
            .Include(p => p.Booking.BookerUser)
            .FirstOrDefaultAsync(p => p.Id == paymentId);
    }
    
    /// <summary>
    /// Updates an existing payment entity in the database.
    /// </summary>
    /// <param name="payment">The payment entity to update.</param>
    /// <returns>The updated payment entity.</returns>
    public async Task<Payment> UpdatePaymentAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        payment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return payment;
    }
}