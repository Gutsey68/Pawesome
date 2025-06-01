using Pawesome.Models.Entities;
using Pawesome.Models.Enums;

namespace Pawesome.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<Payment?> GetPaymentBySessionIdAsync(string sessionId);
    Task<List<Payment>> GetUserPaymentsAsync(int userId);
    Task<Payment?> UpdatePaymentStatusAsync(string sessionId, PaymentStatus status, string? paymentIntentId = null);
    Task<List<Payment>> GetPaymentsByUserAndAdvertAsync(int userId, int advertId);
    public Task<Payment?> GetByIdWithDetailsAsync(int paymentId);
    Task<Payment> UpdatePaymentAsync(Payment payment);
}