using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Payment;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels.Payment;
using Stripe;
using Stripe.Checkout;

namespace Pawesome.Services
{
    /// <summary>
    /// Service responsible for handling payment operations through Stripe
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the PaymentService
        /// </summary>
        /// <param name="paymentRepository">Repository for payment operations</param>
        /// <param name="bookingRepository">Repository for booking operations</param>
        /// <param name="userRepository">Repository for user operations</param>
        /// <param name="notificationService"></param>
        /// <param name="logger">Logger instance</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            ILogger<PaymentService> logger,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new payment record for an advert
        /// </summary>
        /// <param name="userId">ID of the user making the payment</param>
        /// <param name="advertId">ID of the advertisement being paid for</param>
        /// <param name="sessionId">Stripe session ID</param>
        /// <returns>DTO of the created payment</returns>
        public async Task<PaymentDto> CreatePaymentAsync(int userId, int advertId, string sessionId)
        {
            try
            {
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(sessionId);

                if (session == null)
                {
                    throw new InvalidOperationException($"Impossible de récupérer la session Stripe {sessionId}");
                }

                var booking = new Booking
                {
                    AdvertId = advertId,
                    BookerUserId = userId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Status = Models.enums.BookingStatus.PendingConfirmation,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Advert = default!,
                    BookerUser = default!,
                    Payments = default!,
                };
                
                

                var createdBooking = await _bookingRepository.CreateBookingAsync(booking);

                var payment = new Payment
                {
                    Amount = (session.AmountTotal ?? 0) / 100m,
                    Status = PaymentStatus.Pending,
                    SessionId = sessionId,
                    PaymentIntentId = session.PaymentIntentId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    BookingId = createdBooking.Id,
                    Booking = createdBooking
                };

                var createdPayment = await _paymentRepository.CreatePaymentAsync(payment);
                return _mapper.Map<PaymentDto>(createdPayment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors de la création du paiement pour l'utilisateur {UserId}, annonce {AdvertId}, session {SessionId}",
                    userId, advertId, sessionId);
                throw;
            }
        }

        /// <summary>
        /// Completes a payment after Stripe checkout is complete
        /// </summary>
        /// <param name="sessionId">Stripe session ID</param>
        /// <returns>DTO of the updated payment</returns>
        public async Task<PaymentDto?> CompletePaymentAsync(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session == null || string.IsNullOrEmpty(session.ClientReferenceId))
                    return null;

                if (!int.TryParse(session.ClientReferenceId, out var bookingId))
                    return null;

                var booking = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);
                if (booking == null)
                    return null;

                var payment = new Payment
                {
                    BookingId = bookingId,
                    Amount = booking.Amount,
                    PaymentIntentId = session.PaymentIntentId,
                    Status = PaymentStatus.Authorized,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Booking = booking
                };

                await _paymentRepository.CreatePaymentAsync(payment);

                return _mapper.Map<PaymentDto>(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la complétion du paiement pour la session {SessionId}", sessionId);
                return null;
            }
        }

        /// <summary>
        /// Retrieves the payment history for a user
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>List of payment history view models</returns>
        public async Task<List<PaymentHistoryViewModel>> GetUserPaymentsForHistoryAsync(int userId)
        {
            var payments = await _paymentRepository.GetUserPaymentsAsync(userId);
            return _mapper.Map<List<PaymentHistoryViewModel>>(payments);
        }

        /// <summary>
        /// Captures a previously authorized payment
        /// </summary>
        /// <param name="paymentIntentId">Stripe payment intent ID</param>
        /// <returns>True if capture was successful</returns>
        public async Task<bool> CapturePaymentAsync(string paymentIntentId)
        {
            try
            {
                var payment = await GetPaymentByPaymentIntentIdAsync(paymentIntentId);
                if (payment == null || payment.SessionId == null)
                {
                    _logger.LogError("Paiement non trouvé pour PaymentIntentId {PaymentIntentId}", paymentIntentId);
                    return false;
                }

                var booking = payment.Booking;
                if (booking == null)
                {
                    _logger.LogError("Réservation non trouvée pour le paiement avec PaymentIntentId {PaymentIntentId}",
                        paymentIntentId);
                    return false;
                }

                var petSitterId = booking.Advert.UserId;
                var petSitter = await _userRepository.GetByIdAsync(petSitterId);

                if (petSitter == null || string.IsNullOrEmpty(petSitter.StripeAccountId))
                {
                    _logger.LogError("Pet sitter ou compte Stripe non trouvé pour la réservation {BookingId}",
                        booking.Id);
                    return false;
                }

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.CaptureAsync(paymentIntentId);

                long amount = paymentIntent.Amount;
                long commission = (long)(amount * 0.15);
                long transferAmount = amount - commission;

                var transferService = new TransferService();
                await transferService.CreateAsync(new TransferCreateOptions
                {
                    Amount = transferAmount,
                    Currency = "eur",
                    Destination = petSitter.StripeAccountId,
                    SourceTransaction = paymentIntent.LatestChargeId,
                    Description = $"Paiement pour la réservation #{booking.Id}"
                });

                decimal amountInEuros = transferAmount / 100m;
                petSitter.BalanceAccount += amountInEuros;
                await _userRepository.UpdateAsync(petSitter);

                await _paymentRepository.UpdatePaymentStatusAsync(
                    payment.SessionId,
                    PaymentStatus.Captured,
                    paymentIntentId);

                await _bookingRepository.UpdateAdvertStatusBasedOnBookingsAsync(booking.AdvertId);

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Erreur Stripe lors de la capture du paiement {PaymentIntentId}", paymentIntentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la capture du paiement {PaymentIntentId}", paymentIntentId);
                return false;
            }
        }

        /// <summary>
        /// Cancels a payment authorization
        /// </summary>
        /// <param name="paymentIntentId">Stripe payment intent ID</param>
        /// <returns>True if cancellation was successful</returns>
        public async Task<bool> CancelPaymentAuthorizationAsync(string paymentIntentId)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.CancelAsync(paymentIntentId);

                var payment = await GetPaymentByPaymentIntentIdAsync(paymentIntentId);

                if (payment == null || payment.SessionId == null)
                {
                    _logger.LogError("Paiement ou SessionId non trouvé pour PaymentIntentId {PaymentIntentId}",
                        paymentIntentId);
                    return false;
                }

                await _paymentRepository.UpdatePaymentStatusAsync(
                    payment.SessionId,
                    PaymentStatus.Cancelled,
                    paymentIntentId);

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Erreur Stripe lors de l'annulation du paiement {PaymentIntentId}",
                    paymentIntentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'annulation du paiement {PaymentIntentId}", paymentIntentId);
                return false;
            }
        }

        /// <summary>
        /// Retrieves a payment by its associated booking ID
        /// </summary>
        /// <param name="bookingId">ID of the booking</param>
        /// <returns>DTO of the payment</returns>
        public async Task<PaymentDto?> GetPaymentByBookingIdAsync(int bookingId)
        {
            var payments = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);

            if (payments == null || payments.Payments.Count == 0)
            {
                return null;
            }

            var payment = payments.Payments
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
        }

        /// <summary>
        /// Processes Stripe webhooks
        /// </summary>
        /// <param name="json">JSON content of the webhook</param>
        /// <param name="signature">Webhook signature</param>
        /// <param name="webhookSecret">Webhook secret key</param>
        /// <returns>True if webhook was successfully processed</returns>
        public async Task<bool> HandleStripeWebhookAsync(string json, string signature, string webhookSecret)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    webhookSecret
                );

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogInformation("Paiement réussi: {PaymentIntentId}", paymentIntent.Id);

                        var payment = await GetPaymentByPaymentIntentIdAsync(paymentIntent.Id);
                        if (payment?.SessionId != null)
                        {
                            await _paymentRepository.UpdatePaymentStatusAsync(
                                payment.SessionId,
                                PaymentStatus.Captured);
                        }

                        break;

                    case "payment_intent.payment_failed":
                        var failedPaymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogWarning("Échec du paiement: {PaymentIntentId}, {Error}",
                            failedPaymentIntent.Id,
                            failedPaymentIntent.LastPaymentError?.Message);

                        var failedPayment = await GetPaymentByPaymentIntentIdAsync(failedPaymentIntent.Id);
                        if (failedPayment?.SessionId != null)
                        {
                            await _paymentRepository.UpdatePaymentStatusAsync(
                                failedPayment.SessionId,
                                PaymentStatus.Failed);
                        }

                        break;
                }

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement du webhook Stripe");
                return false;
            }
        }

        /// <summary>
        /// Helper method to get a payment by its payment intent ID
        /// </summary>
        private async Task<Payment?> GetPaymentByPaymentIntentIdAsync(string paymentIntentId)
        {
            var allPayments = await _bookingRepository.GetAllAsync();

            foreach (var booking in allPayments)
            {
                var payment = booking.Payments.FirstOrDefault(p => p.PaymentIntentId == paymentIntentId);
                if (payment != null)
                {
                    return payment;
                }
            }

            return null;
        }

        /// <summary>
        /// Finalizes the payment for a booking by marking the payment as captured and updating the advert owner's balance.
        /// This method should be called when a booking is completed and the payment needs to be released to the advert owner.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to finalize payment for.</param>
        /// <returns>True if the payment was successfully finalized, false otherwise.</returns>
        public async Task<bool> FinalizeBookingPaymentAsync(int bookingId)
        {
            try {
                var payment = await _paymentRepository.GetByIdWithDetailsAsync(bookingId);
                if (payment == null || payment.PaymentIntentId == null)
                    return false;

                var service = new PaymentIntentService();
                await service.CaptureAsync(payment.PaymentIntentId, new PaymentIntentCaptureOptions());
        
                payment.Status = PaymentStatus.Completed;
                payment.CreatedAt = DateTime.UtcNow;
                await _paymentRepository.UpdatePaymentAsync(payment);
        
                var booking = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);
                if (booking != null) {
                    await _userRepository.UpdateUserBalanceAsync(booking.Advert.UserId, payment.Amount);
                }
        
                return true;
            } catch (Exception ex) {
                _logger.LogError(ex, "Erreur lors de la finalisation du paiement pour la réservation {BookingId}", bookingId);
                return false;
            }
        }
    }
}