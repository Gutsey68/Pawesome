using Pawesome.Models.Dtos.Payment;
using Pawesome.Models.ViewModels.Payment;

namespace Pawesome.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(int userId, int advertId, string sessionId);
    Task<PaymentDto?> CompletePaymentAsync(string sessionId);
    Task<List<PaymentHistoryViewModel>> GetUserPaymentsForHistoryAsync(int userId);
}