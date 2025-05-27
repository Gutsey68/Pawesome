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
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the PaymentService
        /// </summary>
        /// <param name="paymentRepository">Repository for payment operations</param>
        /// <param name="bookingRepository">Repository for booking operations</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public PaymentService(
            IPaymentRepository paymentRepository,
            IBookingRepository bookingRepository,
            ILogger<PaymentService> logger,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
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
                    throw new InvalidOperationException($"Unable to retrieve Stripe session {sessionId}");
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
                _logger.LogError(ex, "Error creating payment for user {UserId}, advert {AdvertId}, session {SessionId}", userId, advertId, sessionId);
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
                var sessionService = new SessionService();
                var session = await sessionService.GetAsync(sessionId);

                if (session == null)
                    return null;

                var paymentStatus = session.PaymentStatus switch
                {
                    "paid" => PaymentStatus.Authorized,
                    _ => PaymentStatus.Failed
                };

                var payment = await _paymentRepository.UpdatePaymentStatusAsync(
                    sessionId,
                    paymentStatus,
                    session.PaymentIntentId);

                return payment != null ? _mapper.Map<PaymentDto>(payment) : null;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error retrieving session {SessionId}", sessionId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing payment for session {SessionId}", sessionId);
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
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.CaptureAsync(paymentIntentId);
                
                // We need to find the payment by session ID, so first get the session ID
                // from the payment intent data
                var payment = await GetPaymentByPaymentIntentIdAsync(paymentIntentId);
                
                if (payment == null || payment.SessionId == null)
                {
                    _logger.LogError("Payment or SessionId not found for PaymentIntentId {PaymentIntentId}", paymentIntentId);
                    return false;
                }

                await _paymentRepository.UpdatePaymentStatusAsync(
                    payment.SessionId,
                    PaymentStatus.Captured,
                    paymentIntentId);

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error capturing payment {PaymentIntentId}", paymentIntentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing payment {PaymentIntentId}", paymentIntentId);
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
                
                // We need to find the payment by session ID, so first get the session ID
                // from the payment intent data
                var payment = await GetPaymentByPaymentIntentIdAsync(paymentIntentId);
                
                if (payment == null || payment.SessionId == null)
                {
                    _logger.LogError("Payment or SessionId not found for PaymentIntentId {PaymentIntentId}", paymentIntentId);
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
                _logger.LogError(ex, "Stripe error cancelling payment {PaymentIntentId}", paymentIntentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling payment {PaymentIntentId}", paymentIntentId);
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
            
            // Get the most recent payment
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

                // Handle different types of Stripe events
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                        _logger.LogInformation("Payment succeeded: {PaymentIntentId}", paymentIntent.Id);
                        
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
                        _logger.LogWarning("Payment failed: {PaymentIntentId}, {Error}",
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
                _logger.LogError(ex, "Error processing Stripe webhook");
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
    }
}