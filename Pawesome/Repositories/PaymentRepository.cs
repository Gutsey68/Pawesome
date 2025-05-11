using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;

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
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    /// <summary>
    /// Retrieves a payment by its Stripe session ID
    /// </summary>
    /// <param name="sessionId">The Stripe session ID to search for</param>
    /// <returns>The payment entity if found, null otherwise</returns>
    public async Task<Payment?> GetPaymentBySessionIdAsync(string sessionId)
    {
        return await _context.Payments
            .Include(p => p.User)
            .Include(p => p.Advert)
            .FirstOrDefaultAsync(p => p.SessionId == sessionId);
    }

    /// <summary>
    /// Retrieves all payments for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose payments to retrieve</param>
    /// <returns>A list of payment entities for the specified user</returns>
    public async Task<List<Payment>> GetUserPaymentsAsync(int userId)
    {
        return await _context.Payments
            .Include(p => p.Advert)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Updates the status of a payment identified by its session ID
    /// </summary>
    /// <param name="sessionId">The session ID of the payment to update</param>
    /// <param name="status">The new payment status</param>
    /// <param name="paymentIntentId">Optional payment intent ID</param>
    /// <returns>The updated payment entity if found, null otherwise</returns>
    public async Task<Payment?> UpdatePaymentStatusAsync(string sessionId, string status, string? paymentIntentId = null)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.SessionId == sessionId);

        if (payment == null)
            return null;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;

        if (paymentIntentId != null)
            payment.PaymentIntentId = paymentIntentId;

        await _context.SaveChangesAsync();
        return payment;
    }
}