using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<Payment?> GetPaymentBySessionIdAsync(string sessionId);
    Task<List<Payment>> GetUserPaymentsAsync(int userId);
    Task<Payment?> UpdatePaymentStatusAsync(string sessionId, string status, string? paymentIntentId = null);
    Task<List<Payment>> GetPaymentsByUserAndAdvertAsync(int userId, int advertId);
}