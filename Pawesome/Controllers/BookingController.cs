using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using Pawesome.Models.DTOs.Booking;
using Pawesome.Models.Enums;

namespace Pawesome.Controllers
{
    /// <summary>
    /// Controller responsible for handling all booking-related operations.
    /// Provides endpoints for managing the entire booking lifecycle.
    /// </summary>
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IAdvertService _advertService;
        private readonly UserManager<User> _userManager;
        private readonly IStripeBalanceService _balanceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingController"/> class.
        /// </summary>
        /// <param name="bookingService">The service handling booking business logic.</param>
        /// <param name="advertService">The service handling advertisement business logic.</param>
        /// <param name="userManager">The identity user manager.</param>
        public BookingController(
            IBookingService bookingService,
            IAdvertService advertService,
            UserManager<User> userManager,
            IStripeBalanceService balanceService
            )
        {
            _bookingService = bookingService;
            _advertService = advertService;
            _userManager = userManager;
            _balanceService = balanceService;
        }

        /// <summary>
        /// Displays the booking dashboard showing the user's bookings as both a booker and a pet sitter.
        /// </summary>
        /// <returns>A view containing all bookings for the current user.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var bookingsAsBooker = await _bookingService.GetUserBookingsAsync(user.Id, true);
            var bookingsAsPetSitter = await _bookingService.GetUserBookingsAsync(user.Id, false);

            ViewBag.BookingsAsBooker = bookingsAsBooker;
            ViewBag.BookingsAsPetSitter = bookingsAsPetSitter;

            return View();
        }

        /// <summary>
        /// Displays detailed information about a specific booking.
        /// </summary>
        /// <param name="id">The ID of the booking to display.</param>
        /// <returns>A view with detailed information about the booking.</returns>
        /// <remarks>
        /// Only allows access if the current user is either the booker or the pet sitter
        /// associated with the booking.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            if (booking.BookerUserId != user.Id && booking.PetSitterUserId != user.Id)
                return Forbid();

            ViewBag.IsPetSitter = booking.PetSitterUserId == user.Id;

            ViewBag.IsBooker = booking.BookerUserId == user.Id;

            ViewBag.CanIPay = booking.BookerUserId != user.Id &&
                                  booking.AdvertStatus == AdvertStatus.Pending;
            ;

            var advert = await _advertService.GetAdvertByIdAsync(booking.AdvertId);
            bool isAdvertCreatedByPetSitter = advert != null && advert.IsPetSitter;

            ViewBag.CanValidate = (isAdvertCreatedByPetSitter && booking.BookerUserId == user.Id) ||
                                  (!isAdvertCreatedByPetSitter && booking.PetSitterUserId == user.Id) &&
                                  (booking.Status == BookingStatus.Accepted ||
                                   booking.Status == BookingStatus.InProgress ||
                                   booking.Status == BookingStatus.Completed && !booking.IsValidated);

            ViewBag.IsPetSitter = booking.PetSitterUserId == user.Id;
            ViewBag.IsBooker = booking.BookerUserId == user.Id;

            ViewBag.canDispute =booking.Status == BookingStatus.Accepted ||
                                booking.Status == BookingStatus.InProgress ||
                                booking.Status == BookingStatus.Completed && 
                                booking.BookerUserId == user.Id &&
                                !booking.IsDisputed;

            ViewBag.CanUpdateStatus = booking.PetSitterUserId == user.Id &&
                                      booking.Status == BookingStatus.PendingConfirmation;

            return View(booking);
        }

        /// <summary>
        /// Displays the form for creating a new booking for an advertisement.
        /// </summary>
        /// <param name="advertId">The ID of the advertisement to book.</param>
        /// <returns>A view containing the booking creation form.</returns>
        /// <remarks>
        /// Validates that the user is not attempting to book their own advertisement.
        /// Pre-fills the form with information from the advertisement.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> Create(int advertId)
        {
            var advert = await _advertService.GetAdvertByIdAsync(advertId);
            if (advert == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            if (advert.Owner.Id == user.Id)
                return BadRequest("Vous ne pouvez pas réserver votre propre annonce");

            var model = new CreateBookingDto
            {
                AdvertId = advertId,
                StartDate = advert.StartDate,
                EndDate = advert.EndDate
            };

            ViewBag.Advert = advert;
            return View(model);
        }

        /// <summary>
        /// Processes the booking creation form submission.
        /// </summary>
        /// <param name="model">The booking information submitted by the user.</param>
        /// <returns>
        /// If successful, redirects to the payment checkout page.
        /// If validation fails, returns the form with error messages.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto model)
        {
            if (!ModelState.IsValid)
            {
                var advert = await _advertService.GetAdvertByIdAsync(model.AdvertId);
                ViewBag.Advert = advert;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            try
            {
                var booking = await _bookingService.CreateBookingAsync(model, user.Id);

                var advert = await _advertService.GetAdvertByIdAsync(model.AdvertId);
                if (advert != null && !advert.IsPetSitter)
                {
                    return RedirectToAction(nameof(Details), new { id = booking.Id });
                }

                return RedirectToAction("Checkout", "Payment", new { bookingId = booking.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var advert = await _advertService.GetAdvertByIdAsync(model.AdvertId);
                ViewBag.Advert = advert;
                return View(model);
            }
        }

        /// <summary>
        /// Updates the status of a booking (accept, decline, etc.).
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="status">The new status to set for the booking.</param>
        /// <returns>
        /// If successful, redirects to the booking details page.
        /// If the operation fails, returns a bad request response.
        /// </returns>
        /// <remarks>
        /// Only the pet sitter associated with the booking can update its status.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int bookingId, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound();

            if (booking.PetSitterUserId != user.Id)
                return Forbid();

            if (!Enum.TryParse<BookingStatus>(status, true, out var bookingStatus))
                return BadRequest("Statut invalide");

            var result = await _bookingService.UpdateBookingStatusAsync(bookingId, bookingStatus);
            if (!result)
                return BadRequest("La mise à jour du statut a échoué");

            return RedirectToAction(nameof(Details), new { id = bookingId });
        }

        /// <summary>
        /// Validates a completed booking service, confirming that the service was performed satisfactorily.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to validate.</param>
        /// <returns>
        /// If successful, redirects to the booking details page.
        /// If the operation fails, returns a bad request response.
        /// </returns>
        /// <remarks>
        /// Only the booker (service recipient) can validate a booking.
        /// This action triggers payment capture in Stripe.
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound();
            
            var advert = await _advertService.GetAdvertByIdAsync(booking.AdvertId);
            if (advert == null)
                return NotFound();

            bool isAdvertCreatedByPetSitter = advert.IsPetSitter;
            bool canValidate = (isAdvertCreatedByPetSitter && booking.BookerUserId == user.Id) ||
                               (!isAdvertCreatedByPetSitter && booking.PetSitterUserId == user.Id);

            if (!canValidate)
                return Forbid();

            var result = await _bookingService.ValidateBookingAsync(bookingId);
            if (result)
            {
                TempData["SuccessMessage"] = "La réservation a été validée avec succès et le paiement a été finalisé.";
            }
            else
            {
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la validation de la réservation.";
            }
            
            await _advertService.UpdateAdvertStatusAsync(booking.AdvertId, AdvertStatus.FullyBooked);

            return RedirectToAction(nameof(Details), new { id = bookingId });
        }

        /// <summary>
        /// Displays the form for submitting a dispute for a booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to dispute.</param>
        /// <returns>A view containing the dispute submission form.</returns>
        /// <remarks>
        /// Only the booker (service recipient) can raise a dispute for a booking.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> Dispute(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound();

            if (booking.BookerUserId != user.Id)
                return Forbid();

            ViewBag.BookingId = bookingId;
            return View();
        }

        /// <summary>
        /// Processes a booking dispute submission.
        /// </summary>
        /// <param name="bookingId">The ID of the booking being disputed.</param>
        /// <param name="reason">The reason for the dispute.</param>
        /// <returns>
        /// If successful, redirects to the booking details page.
        /// If validation fails, returns the form with error messages.
        /// </returns>
        /// <remarks>
        /// Only the booker (service recipient) can submit a dispute.
        /// The dispute reason is required and cannot be empty.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Dispute(int bookingId, string reason)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();

            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound();

            if (booking.BookerUserId != user.Id)
                return Forbid();

            if (string.IsNullOrEmpty(reason))
            {
                ModelState.AddModelError("", "Veuillez indiquer la raison du litige");
                ViewBag.BookingId = bookingId;
                return View();
            }

            var result = await _bookingService.DisputeBookingAsync(bookingId, reason);
            if (!result)
                return BadRequest("L'ouverture du litige a échoué");

            return RedirectToAction(nameof(Details), new { id = bookingId });
        }
        
        /// <summary>
        /// Changes the status of an advert and initiates payment if fully booked.
        /// </summary>
        /// <param name="id">The ID of the advert to update.</param>
        /// <param name="status">The new status to set for the advert.</param>
        /// <returns>
        /// If successful, redirects to the payment checkout page if the advert is fully booked,
        /// otherwise redirects to the advert index. If the advert is not found or the user is not authorized,
        /// returns the appropriate error response.
        /// </returns>
        /// <remarks>
        /// Only the owner of the advert can change its status. If the status is set to FullyBooked,
        /// the user is redirected to the payment checkout for the associated booking.
        /// </remarks>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeStatusAndPay(int id, AdvertStatus status)
        {
            var advert = await _advertService.GetAdvertByIdAsync(id);

            if (advert == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || advert.Owner.Id != user.Id)
            {
                return Forbid();
            }

            var success = await _advertService.UpdateAdvertStatusAsync(id, status);

            if (!success)
            {
                TempData["ErrorMessage"] = "Échec de la mise à jour du statut de l'annonce.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Le statut de l'annonce a été mis à jour avec succès.";

            if (status == AdvertStatus.FullyBooked)
            {
                var booking = await _bookingService.GetBookingByAdvertIdAsync(id);
        
                if (booking != null)
                {
                    return RedirectToAction("Checkout", "Payment", new { bookingId = booking.Id });
                }
            }

            return RedirectToAction("Index");
        }
    }
}