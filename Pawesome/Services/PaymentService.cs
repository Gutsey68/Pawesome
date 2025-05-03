using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Payment;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Payment;
using Stripe.Checkout;

namespace Pawesome.Services;

/// <summary>
/// Service responsible for handling payment-related business logic
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAdvertService _advertService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAdvertRepository _advertRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentService"/> class
    /// </summary>
    /// <param name="repository">Repository for payment operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    /// <param name="advertService">Service for managing advertisements</param>
    /// <param name="paymentRepository">Repository for payment operations</param>
    /// <param name="userRepository">Repository for user operations</param>
    /// <param name="advertRepository">Repository for advertisement operations</param>
    public PaymentService(
        IPaymentRepository repository,
        IMapper mapper,
        IAdvertService advertService,
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        IAdvertRepository advertRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _advertService = advertService;
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _advertRepository = advertRepository;
    }

    /// <summary>
    /// Creates a new payment record for a specific advert and user
    /// </summary>
    /// <param name="userId">The ID of the user making the payment</param>
    /// <param name="advertId">The ID of the advert being paid for</param>
    /// <param name="sessionId">The Stripe session ID associated with the payment</param>
    /// <returns>The created payment data transfer object</returns>
    /// <exception cref="InvalidOperationException">Thrown when the advert cannot be found</exception>
    /// <exception cref="ArgumentException">Thrown when the user cannot be found</exception>
    public async Task<PaymentDto> CreatePaymentAsync(int userId, int advertId, string sessionId)
    {
        var advert = await _advertRepository.GetAdvertByIdAsync(advertId);

        if (advert == null)
        {
            throw new InvalidOperationException($"L'annonce avec l'ID {advertId} est introuvable");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"L'utilisateur avec l'ID {userId} n'a pas été trouvé");
        }

        var payment = new Payment
        {
            UserId = userId,
            AdvertId = advertId,
            Amount = advert.Amount,
            Status = "pending",
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = user,
            Advert = advert
        };

        var createdPayment = await _repository.CreatePaymentAsync(payment);
        return _mapper.Map<PaymentDto>(createdPayment);
    }

    /// <summary>
    /// Completes a payment process by updating its status based on Stripe session information
    /// </summary>
    /// <param name="sessionId">The Stripe session ID of the payment to complete</param>
    /// <returns>The updated payment data transfer object if successful, null otherwise</returns>
    public async Task<PaymentDto?> CompletePaymentAsync(string sessionId)
    {
        var sessionService = new SessionService();
        var session = await sessionService.GetAsync(sessionId);

        if (session == null)
            return null;

        var payment = await _repository.UpdatePaymentStatusAsync(
            sessionId,
            session.PaymentStatus == "paid" ? "completed" : "failed",
            session.PaymentIntentId);

        if (payment != null && payment.Status == "completed")
        {
            await _advertService.UpdateAdvertStatusAsync(payment.AdvertId, "reserved");
        }

        return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
    }

    /// <summary>
    /// Retrieves payment history for a specific user in a format suitable for display
    /// </summary>
    /// <param name="userId">The ID of the user whose payment history to retrieve</param>
    /// <returns>A list of payment history view models</returns>
    public async Task<List<PaymentHistoryViewModel>> GetUserPaymentsForHistoryAsync(int userId)
    {
        var payments = await _repository.GetUserPaymentsAsync(userId);
        
        return _mapper.Map<List<PaymentHistoryViewModel>>(payments);
    }
}