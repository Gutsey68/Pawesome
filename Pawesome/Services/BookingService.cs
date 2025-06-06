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
        private readonly IStripeBalanceService _balanceService;
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
        /// <param name="balanceService">Service for balance operations</param>
        /// <param name="userRepository">Repository for user operations</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        /// <param name="logger">Logger instance</param>
        public BookingService(
            IBookingRepository bookingRepository,
            IAdvertService advertService,
            IPaymentService paymentService,
            IStripeBalanceService balanceService,
            INotificationService notificationService,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _advertService = advertService;
            _paymentService = paymentService;
            _balanceService = balanceService;
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
                _logger.LogError(ex, "Erreur lors de la récupération des réservations pour l'utilisateur {UserId}",
                    userId);
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

                if (createDto.EndDate.Date <= createDto.StartDate.Date)
                {
                    throw new InvalidOperationException("La date de fin doit être après la date de début.");
                }

                if (createDto.StartDate.Date < advert.StartDate.Date || createDto.EndDate.Date > advert.EndDate.Date)
                {
                    throw new InvalidOperationException(
                        "Les dates choisies sont en dehors de la disponibilité du pet sitter.");
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
                _logger.LogError(ex,
                    "Erreur lors de la création d'une réservation pour l'annonce {AdvertId} par l'utilisateur {UserId}",
                    createDto.AdvertId, userId);
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

                var advertId = booking.AdvertId;
                var advert = await _advertService.GetAdvertByIdAsync(advertId);
                if (advert == null)
                {
                    return false;
                }

                var result = await _bookingRepository.UpdateBookingStatusAsync(bookingId, status);

                if (result)
                {
                    await UpdateAdvertStatusAsync(advertId);

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
                        string message = advert.IsPetSitter
                            ? "Le pet sitter a accepté votre demande de réservation."
                            : "Le propriétaire d'animal a accepté votre offre de service.";

                        await CreateAndSendNotificationAsync(
                            booking.BookerUserId,
                            NotificationType.BookingStatusChanged,
                            $"Réservation #{bookingId} acceptée",
                            message
                        );
                    }
                    else if (status == BookingStatus.Declined)
                    {
                        string message = advert.IsPetSitter
                            ? "Le pet sitter a refusé votre demande de réservation."
                            : "Le propriétaire d'animal a refusé votre offre de service.";

                        await CreateAndSendNotificationAsync(
                            booking.BookerUserId,
                            NotificationType.BookingStatusChanged,
                            $"Réservation #{bookingId} refusée",
                            message
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
                        string message = advert.IsPetSitter
                            ? "La garde de votre animal est terminée. N'oubliez pas de valider la prestation."
                            : "La prestation est terminée. Attendez la validation par le pet sitter.";

                        await CreateAndSendNotificationAsync(
                            booking.BookerUserId,
                            NotificationType.BookingStatusChanged,
                            $"Réservation #{bookingId} terminée",
                            message
                        );
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut de la réservation {BookingId} à {Status}",
                    bookingId, status);
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
            _logger.LogWarning("Validation impossible : réservation {BookingId} non trouvée", bookingId);
            return false;
        }

        _logger.LogInformation("Début de validation de la réservation {BookingId} pour le montant {Amount}€",
            bookingId, booking.Amount);

        var payment = booking.Payments
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefault(p => p.Status == PaymentStatus.Authorized);

        if (payment != null)
        {
            _logger.LogInformation("Finalisation du paiement {PaymentId} pour la réservation {BookingId}",
                payment.Id, bookingId);
                
            var paymentSuccess = await _paymentService.FinalizeBookingPaymentAsync(bookingId);

            if (!paymentSuccess)
            {
                _logger.LogError("Échec de la finalisation du paiement pour la réservation {BookingId}",
                    bookingId);
                return false;
            }
            
        }
        else
        {
            _logger.LogInformation(
                "Aucun paiement à finaliser pour la réservation {BookingId} ou paiement déjà complété",
                bookingId);
        }

        var result = await _bookingRepository.ValidateBookingAsync(bookingId);
        _logger.LogInformation("Résultat de la validation de la réservation {BookingId} : {Result}", bookingId,
            result);

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
                    await UpdateAdvertStatusAsync(booking.AdvertId);

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
                _logger.LogError(ex, "Erreur lors de l'ouverture d'un litige pour la réservation {BookingId}",
                    bookingId);
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
                var validationDeadline = DateTime.UtcNow.AddDays(-3);
                var bookingsToValidate =
                    await _bookingRepository.GetBookingsToAutomaticallyValidateAsync(validationDeadline);

                if (!bookingsToValidate.Any())
                    return true;

                _logger.LogInformation("{Count} réservation(s) à valider automatiquement", bookingsToValidate.Count);

                foreach (var booking in bookingsToValidate)
                {
                    var payment = booking.Payments.FirstOrDefault(p => p.Status == PaymentStatus.Authorized);

                    if (payment != null)
                    {
                        var success = await _paymentService.FinalizeBookingPaymentAsync(payment.Id);

                        if (success)
                        {
                            _logger.LogInformation(
                                "Validation automatique réussie pour la réservation {BookingId}, paiement {PaymentId}",
                                booking.Id, payment.Id);

                            await _bookingRepository.ValidateBookingAsync(booking.Id);

                            await _notificationService.CreateNotificationAsync(new Notification
                            {
                                UserId = booking.BookerUserId,
                                Title = "Réservation validée automatiquement",
                                Message =
                                    $"Votre réservation du {booking.StartDate:dd/MM/yyyy} au {booking.EndDate:dd/MM/yyyy} a été validée automatiquement.",
                                Type = NotificationType.BookingStatusChange,
                                IsRead = false,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                User = booking.BookerUser
                            });

                            await _notificationService.CreateNotificationAsync(new Notification
                            {
                                UserId = booking.Advert.UserId,
                                Title = "Paiement reçu",
                                Message =
                                    $"Le paiement pour la réservation du {booking.StartDate:dd/MM/yyyy} au {booking.EndDate:dd/MM/yyyy} a été effectué.",
                                Type = NotificationType.BookingValidated,
                                IsRead = false,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                User = booking.Advert.User
                            });
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Échec de la validation automatique pour la réservation {BookingId}, paiement {PaymentId}",
                                booking.Id, payment.Id);
                        }
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
        private async Task<Notification> CreateAndSendNotificationAsync(int userId, NotificationType type, string title,
            string message)
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

        /// <summary>
        /// Gets the total count of bookings in the system
        /// </summary>
        /// <returns>The total number of bookings</returns>
        public int GetBookingsCount()
        {
            return _bookingRepository.GetAllAsync().Result.Count();
        }

        /// <summary>
        /// Gets all bookings in the system
        /// </summary>
        /// <returns>List of all booking DTOs</returns>
        public List<BookingDto> GetAllBookings()
        {
            var bookings = _bookingRepository.GetAllAsync();
            return _mapper.Map<List<BookingDto>>(bookings);
        }

        /// <summary>
        /// Updates the status of a booking automatically based on start and end dates
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update</param>
        /// <returns>True if update was successful, false otherwise</returns>
        public async Task<bool> UpdateBookingStatusAutomaticallyAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByIdWithDetailsAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }

                var today = DateTime.UtcNow;
                var currentStatus = booking.Status;
                var hasChanged = false;

                if (booking.Status == BookingStatus.Accepted && today.Date >= booking.StartDate.Date)
                {
                    booking.Status = BookingStatus.InProgress;
                    booking.UpdatedAt = today;
                    hasChanged = true;

                    await CreateAndSendNotificationAsync(
                        booking.BookerUserId,
                        NotificationType.BookingStatusChanged,
                        $"Réservation commencée",
                        "La garde de votre animal a commencé automatiquement selon les dates prévues."
                    );
                }
                else if (booking.Status == BookingStatus.InProgress && today.Date > booking.EndDate.Date)
                {
                    booking.Status = BookingStatus.Completed;
                    booking.UpdatedAt = today;
                    hasChanged = true;

                    await CreateAndSendNotificationAsync(
                        booking.BookerUserId,
                        NotificationType.BookingStatusChanged,
                        $"Réservation terminée",
                        "La garde de votre animal est terminée selon les dates prévues. N'oubliez pas de valider la prestation."
                    );
                }

                if (hasChanged)
                {
                    await _bookingRepository.UpdateAsync(booking);
                    _logger.LogInformation(
                        "Statut de la réservation {BookingId} mis à jour automatiquement de {OldStatus} à {NewStatus}",
                        bookingId, currentStatus, booking.Status);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erreur lors de la mise à jour automatique du statut de la réservation {BookingId}", bookingId);
                return false;
            }
        }

        /// <summary>
        /// Updates the status of all active bookings based on their start and end dates
        /// </summary>
        /// <returns>The number of bookings updated</returns>
        public async Task<int> UpdateAllBookingsStatusAutomaticallyAsync()
        {
            try
            {
                var activeBookings = await _bookingRepository.GetActiveBookingsAsync();
                int updatedCount = 0;
                var updatedAdvertIds = new HashSet<int>();

                foreach (var booking in activeBookings)
                {
                    var result = await UpdateBookingStatusAutomaticallyAsync(booking.Id);
                    if (result)
                    {
                        updatedCount++;
                        updatedAdvertIds.Add(booking.AdvertId);
                    }
                }

                foreach (var advertId in updatedAdvertIds)
                {
                    await UpdateAdvertStatusAsync(advertId);
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour automatique des statuts de réservation");
                return 0;
            }
        }

        /// <summary>
        /// Updates the status of an advert based on its associated bookings.
        /// </summary>
        /// <param name="advertId">The ID of the advert to update.</param>
        /// <returns>True if the advert status was updated successfully, false otherwise.</returns>
        /// <remarks>
        /// - If there are active bookings (Accepted or InProgress), the advert is set to FullyBooked if all periods are covered, otherwise Active.
        /// - If there are no active bookings and the advert end date is in the past, the advert is set to Expired.
        /// - If the advert was previously FullyBooked but is no longer, it is set back to Active.
        /// </remarks>
        private async Task<bool> UpdateAdvertStatusAsync(int advertId)
        {
            try
            {
                return await _bookingRepository.UpdateAdvertStatusBasedOnBookingsAsync(advertId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut de l'annonce {AdvertId}", advertId);
                return false;
            }
        }

        /// <summary>
        /// Retrieves the first booking associated with a specific advert.
        /// </summary>
        /// <param name="advertId">The ID of the advert.</param>
        /// <returns>The first booking DTO for the given advert, or null if not found.</returns>
        public async Task<BookingDto?> GetBookingByAdvertIdAsync(int advertId)
        {
            var booking = await _bookingRepository.GetBookingByAdvertIdAsync(advertId);
            return booking != null ? _mapper.Map<BookingDto>(booking) : null;
        }
    }
}