using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Booking;
using Pawesome.Models.DTOs.Booking;
using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using Pawesome.Models.Enums;

namespace Pawesome.Services
{
    /// <summary>
    /// Service responsible for managing pet sitting bookings
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IAdvertService _advertService;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        /// <summary>
        /// Initializes a new instance of the BookingService
        /// </summary>
        /// <param name="bookingRepository">Repository for booking operations</param>
        /// <param name="advertService">Service for advert operations</param>
        /// <param name="paymentService">Service for payment operations</param>
        /// <param name="notificationService">Service for notification operations</param>
        /// <param name="userRepository">Repository for user operations</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        /// <param name="logger">Logger instance</param>
        public BookingService(
            IBookingRepository bookingRepository,
            IAdvertService advertService,
            IPaymentService paymentService,
            INotificationService notificationService,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _advertService = advertService;
            _paymentService = paymentService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a booking by its ID
        /// </summary>
        /// <param name="bookingId">The ID of the booking to retrieve</param>
        /// <returns>The booking DTO if found, null otherwise</returns>
        public async Task<BookingDto?> GetBookingByIdAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);
                if (booking == null)
                {
                    return null;
                }
                
                return _mapper.Map<BookingDto>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la réservation {BookingId}", bookingId);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all bookings for a user, either as a pet owner (booker) or pet sitter
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="asBooker">If true, retrieves bookings where user is the booker, otherwise where user is the pet sitter</param>
        /// <returns>List of booking DTOs</returns>
        public async Task<List<BookingDto>> GetUserBookingsAsync(int userId, bool asBooker)
        {
            try
            {
                var bookings = await _bookingRepository.GetBookingsByUserIdAsync(userId, asBooker);
                return _mapper.Map<List<BookingDto>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des réservations pour l'utilisateur {UserId}", userId);
                return new List<BookingDto>();
            }
        }

        /// <summary>
        /// Creates a new booking request
        /// </summary>
        /// <param name="createDto">The data for creating the booking</param>
        /// <param name="userId">The ID of the user creating the booking</param>
        /// <returns>The created booking DTO</returns>
        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createDto, int userId)
        {
            try
            {
                var advert = await _advertService.GetAdvertByIdAsync(createDto.AdvertId);
                if (advert == null)
                {
                    throw new InvalidOperationException("L'annonce demandée n'existe pas.");
                }
                
                if (advert.Owner.Id == userId)
                {
                    throw new InvalidOperationException("Vous ne pouvez pas réserver votre propre annonce.");
                }
                
                if (createDto.StartDate < DateTime.UtcNow)
                {
                    throw new InvalidOperationException("La date de début doit être dans le futur.");
                }
                
                if (createDto.EndDate <= createDto.StartDate)
                {
                    throw new InvalidOperationException("La date de fin doit être après la date de début.");
                }
                
                if (createDto.StartDate < advert.StartDate || createDto.EndDate > advert.EndDate)
                {
                    throw new InvalidOperationException("Les dates choisies sont en dehors de la disponibilité du pet sitter.");
                }
                
                var advertBookings = await _bookingRepository.GetBookingsByAdvertIdAsync(advert.Id);
                var overlappingBooking = advertBookings.FirstOrDefault(b => 
                    (b.Status == BookingStatus.Accepted || b.Status == BookingStatus.InProgress) && 
                    ((createDto.StartDate >= b.StartDate && createDto.StartDate < b.EndDate) || 
                     (createDto.EndDate > b.StartDate && createDto.EndDate <= b.EndDate) ||
                     (createDto.StartDate <= b.StartDate && createDto.EndDate >= b.EndDate)));
                
                if (overlappingBooking != null)
                {
                    throw new InvalidOperationException("Le pet sitter a déjà une réservation pour cette période.");
                }
                
                var numberOfDays = (createDto.EndDate - createDto.StartDate).Days;
                var totalAmount = numberOfDays * advert.Amount;
                
                var booking = _mapper.Map<Booking>(createDto);
                booking.BookerUserId = userId;
                booking.Amount = totalAmount;
                
                var createdBooking = await _bookingRepository.CreateBookingAsync(booking);
                
                await CreateAndSendNotificationAsync(
                    advert.Owner.Id,
                    NotificationType.NewBookingRequest,
                    "Nouvelle demande de réservation",
                    $"Vous avez reçu une nouvelle demande de réservation du {createDto.StartDate.ToString("dd/MM/yyyy")} au {createDto.EndDate.ToString("dd/MM/yyyy")}."
                );
                
                return _mapper.Map<BookingDto>(createdBooking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création d'une réservation pour l'annonce {AdvertId} par l'utilisateur {UserId}", createDto.AdvertId, userId);
                throw;
            }
        }

        /// <summary>
        /// Updates the status of a booking
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update</param>
        /// <param name="status">The new status for the booking</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus status)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }

                var result = await _bookingRepository.UpdateBookingStatusAsync(bookingId, status);
                
                if (result)
                {
                    var statusMessage = status switch
                    {
                        BookingStatus.Accepted => "acceptée",
                        BookingStatus.Declined => "refusée",
                        BookingStatus.Cancelled => "annulée",
                        BookingStatus.InProgress => "en cours",
                        BookingStatus.Completed => "terminée",
                        _ => "mise à jour"
                    };
                    
                    if (status == BookingStatus.Accepted)
                    {
                        await CreateAndSendNotificationAsync(
                            booking.BookerUserId,
                            NotificationType.BookingStatusChanged,
                            $"Réservation #{bookingId} acceptée",
                            "Le pet sitter a accepté votre demande de réservation."
                        );
                    }
                    else if (status == BookingStatus.Declined)
                    {
                        await CreateAndSendNotificationAsync(
                            booking.BookerUserId,
                            NotificationType.BookingStatusChanged,
                            $"Réservation #{bookingId} refusée",
                            "Le pet sitter a refusé votre demande de réservation."
                        );
                        
                        var payment = booking.Payments
                            .OrderByDescending(p => p.CreatedAt)
                            .FirstOrDefault();
                        
                        if (payment?.PaymentIntentId != null)
                        {
                            await _paymentService.CancelPaymentAuthorizationAsync(payment.PaymentIntentId);
                        }
                    }
                    else if (status == BookingStatus.InProgress)
                    {
                        await CreateAndSendNotificationAsync(
                            booking.BookerUserId,
                            NotificationType.BookingStatusChanged,
                            $"Réservation #{bookingId} commencée",
                            "La garde de votre animal a commencé."
                        );
                    }
                    else if (status == BookingStatus.Completed)
                    {
                        await CreateAndSendNotificationAsync(
                            booking.BookerUserId,
                            NotificationType.BookingStatusChanged,
                            $"Réservation #{bookingId} terminée",
                            "La garde de votre animal est terminée. N'oubliez pas de valider la prestation."
                        );
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut de la réservation {BookingId} à {Status}", bookingId, status);
                return false;
            }
        }

        /// <summary>
        /// Validates a booking after service completion
        /// </summary>
        /// <param name="bookingId">The ID of the booking to validate</param>
        /// <returns>True if validation was successful, false otherwise</returns>
        public async Task<bool> ValidateBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }
                
                var result = await _bookingRepository.ValidateBookingAsync(bookingId);
                
                if (result)
                {
                    var payment = booking.Payments
                        .OrderByDescending(p => p.CreatedAt)
                        .FirstOrDefault();
                    
                    if (payment?.PaymentIntentId != null)
                    {
                        await _paymentService.CapturePaymentAsync(payment.PaymentIntentId);
                    }
                    
                    await CreateAndSendNotificationAsync(
                        booking.Advert.UserId,
                        NotificationType.BookingValidated,
                        $"Réservation #{bookingId} validée",
                        "Le client a validé la prestation. Votre paiement a été traité."
                    );
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la validation de la réservation {BookingId}", bookingId);
                return false;
            }
        }

        /// <summary>
        /// Opens a dispute for a booking
        /// </summary>
        /// <param name="bookingId">The ID of the booking</param>
        /// <param name="reason">The reason for the dispute</param>
        /// <returns>True if dispute was successfully opened, false otherwise</returns>
        public async Task<bool> DisputeBookingAsync(int bookingId, string reason)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }

                var result = await _bookingRepository.DisputeBookingAsync(bookingId, reason);
                
                if (result)
                {
                    await CreateAndSendNotificationAsync(
                        booking.BookerUserId,
                        NotificationType.BookingDisputed,
                        $"Votre réservation #{bookingId} a été signalée en litige.",
                        $"La réservation a été mise en litige. Motif : {reason}"
                    );
                    
                    await CreateAndSendNotificationAsync(
                        booking.Advert.UserId,
                        NotificationType.BookingDisputed,
                        $"La réservation #{bookingId} a été signalée en litige.",
                        $"Un client a signalé un problème avec cette réservation. Motif : {reason}"
                    );
                    
                    const int adminUserId = 1; //TODO: fix this 
                    await CreateAndSendNotificationAsync(
                        adminUserId,
                        NotificationType.AdminAlert,
                        $"Litige pour la réservation #{bookingId}",
                        $"Un litige a été ouvert. Motif : {reason}. Veuillez examiner ce cas."
                    );
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ouverture d'un litige pour la réservation {BookingId}", bookingId);
                return false;
            }
        }

        /// <summary>
        /// Processes automatic validations for bookings that have ended but haven't been validated
        /// </summary>
        /// <returns>True if processing was successful, false otherwise</returns>
        public async Task<bool> ProcessAutomaticValidationsAsync()
        {
            try
            {
                // Get bookings that should be automatically validated
                var validationDeadline = DateTime.UtcNow.AddDays(-3);
                var bookingsToValidate = await _bookingRepository.GetBookingsToAutomaticallyValidateAsync(validationDeadline);

                foreach (var booking in bookingsToValidate)
                {
                    booking.Status = BookingStatus.Completed;
                    booking.IsValidated = true;
                    booking.ValidatedAt = DateTime.UtcNow;
                    booking.UpdatedAt = DateTime.UtcNow;

                    var payment = booking.Payments
                        .OrderByDescending(p => p.CreatedAt)
                        .FirstOrDefault();

                    if (payment?.PaymentIntentId != null)
                    {
                        await _paymentService.CapturePaymentAsync(payment.PaymentIntentId);
                    }

                    await CreateAndSendNotificationAsync(
                        booking.BookerUserId,
                        NotificationType.BookingValidated,
                        $"Validation automatique de la réservation #{booking.Id}",
                        "La réservation a été validée automatiquement après 3 jours sans action."
                    );
                    
                    await CreateAndSendNotificationAsync(
                        booking.Advert.UserId,
                        NotificationType.BookingValidated,
                        $"Validation automatique de la réservation #{booking.Id}",
                        "La réservation a été validée automatiquement et votre paiement a été traité."
                    );
                }

                if (bookingsToValidate.Any())
                {
                    foreach (var booking in bookingsToValidate)
                    {
                        await _bookingRepository.UpdateAsync(booking);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement des validations automatiques");
                return false;
            }
        }

        /// <summary>
        /// Processes expired bookings (pending confirmation but no response)
        /// </summary>
        /// <returns>True if processing was successful, false otherwise</returns>
        public async Task<bool> ProcessExpiredBookingsAsync()
        {
            try
            {
                var expirationDate = DateTime.UtcNow.AddDays(-3);
                var expiredBookings = await _bookingRepository.GetExpiredBookingsAsync(expirationDate);

                foreach (var booking in expiredBookings)
                {
                    booking.Status = BookingStatus.Declined;
                    booking.UpdatedAt = DateTime.UtcNow;

                    var payment = booking.Payments
                        .OrderByDescending(p => p.CreatedAt)
                        .FirstOrDefault();

                    if (payment?.PaymentIntentId != null)
                    {
                        await _paymentService.CancelPaymentAuthorizationAsync(payment.PaymentIntentId);
                    }

                    await CreateAndSendNotificationAsync(
                        booking.BookerUserId,
                        NotificationType.BookingExpired,
                        $"Réservation #{booking.Id} expirée",
                        "Votre demande de réservation a expiré faute de réponse du pet sitter."
                    );
                }

                if (expiredBookings.Any())
                {
                    foreach (var booking in expiredBookings)
                    {
                        await _bookingRepository.UpdateAsync(booking);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement des réservations expirées");
                return false;
            }
        }
        
        /// <summary>
        /// Helper method to create and send a notification
        /// </summary>
        private async Task<Notification> CreateAndSendNotificationAsync(int userId, NotificationType type, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                User = null!
            };
            
            return await _notificationService.CreateNotificationAsync(notification);
        }
        
        /// <summary>
        /// Gets all pending bookings for adverts owned by the specified user
        /// </summary>
        /// <param name="userId">The ID of the user who owns the adverts</param>
        /// <returns>List of pending booking DTOs</returns>
        public async Task<List<BookingDto>> GetPendingBookingsForUserAdvertsAsync(int userId)
        {
            var bookings = await _bookingRepository.GetPendingBookingsForUserAdvertsAsync(userId);
            return _mapper.Map<List<BookingDto>>(bookings);
        }
    }
}
