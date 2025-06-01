using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pawesome.Interfaces;
using Pawesome.Models.Configuration;

namespace Pawesome.Controllers;

/// <summary>
/// Controller for handling Stripe webhook events and payment-related operations
/// </summary>
[ApiController]
[Route("api/stripe")]
public class WebhookController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly IBookingService _bookingService;
    private readonly string _webhookSecret;

    /// <summary>
    /// Initializes a new instance of the WebhookController
    /// </summary>
    /// <param name="paymentService">The payment service for processing transactions</param>
    /// <param name="bookingService">The booking service for managing reservations</param>
    /// <param name="stripeSettings">Configuration settings for Stripe</param>
    public WebhookController(
        IPaymentService paymentService, 
        IBookingService bookingService,
        IOptions<StripeSettings> stripeSettings)
    {
        _paymentService = paymentService;
        _bookingService = bookingService;
        _webhookSecret = stripeSettings.Value.WebhookSecret;
    }

    /// <summary>
    /// Handles incoming Stripe webhook events
    /// </summary>
    /// <returns>OK if webhook was processed successfully, otherwise BadRequest</returns>
    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Handle()
    {
        using var reader = new StreamReader(HttpContext.Request.Body);
        var json = await reader.ReadToEndAsync();

        var stripeSignature = Request.Headers["Stripe-Signature"];

        var isValid = await _paymentService.HandleStripeWebhookAsync(json, stripeSignature!, _webhookSecret);

        return isValid ? Ok() : BadRequest();
    }
    
    /// <summary>
    /// Validates a booking and captures the payment
    /// </summary>
    /// <param name="bookingId">The ID of the booking to validate</param>
    /// <returns>Redirect to booking details page</returns>
    [Authorize]
    [HttpPost]
    [Route("validate/{bookingId}")] 
    [Route("Webhook/ValidateBooking")] 
    public async Task<IActionResult> ValidateBooking(int bookingId)
    {
        var success = await _bookingService.ValidateBookingAsync(bookingId);
    
        if (success)
        {
            TempData["SuccessMessage"] = "Le service a été validé et le paiement capturé avec succès.";
        }
        else
        {
            TempData["ErrorMessage"] = "Une erreur s'est produite lors de la validation du service.";
        }
    
        return RedirectToAction("Details", "Booking", new { id = bookingId });
    }
    
    /// <summary>
    /// Cancels a booking and the associated payment authorization
    /// </summary>
    /// <param name="bookingId">The ID of the booking to cancel</param>
    /// <returns>Redirect to booking details page</returns>
    [Authorize]
    [HttpPost("cancel/{bookingId}")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        var payment = await _paymentService.GetPaymentByBookingIdAsync(bookingId);
        if (payment == null || payment.PaymentIntentId == null)
            return NotFound();

        var success = await _paymentService.CancelPaymentAuthorizationAsync(payment.PaymentIntentId);
        
        if (success)
        {
            await _bookingService.UpdateBookingStatusAsync(bookingId, Models.enums.BookingStatus.Cancelled);
            
            TempData["SuccessMessage"] = "La réservation a été annulée avec succès.";
            return RedirectToAction("Details", "Booking", new { id = bookingId });
        }
        else
        {
            TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'annulation de la réservation.";
            return RedirectToAction("Details", "Booking", new { id = bookingId });
        }
    }
    
    /// <summary>
    /// Files a dispute for a booking
    /// </summary>
    /// <param name="bookingId">The ID of the booking to dispute</param>
    /// <param name="reason">The reason for the dispute</param>
    /// <returns>Redirect to booking details page</returns>
    [Authorize]
    [HttpPost("dispute/{bookingId}")]
    public async Task<IActionResult> DisputeBooking(int bookingId, string reason)
    {
        if (string.IsNullOrEmpty(reason))
        {
            TempData["ErrorMessage"] = "Veuillez fournir une raison pour le litige.";
            return RedirectToAction("Details", "Booking", new { id = bookingId });
        }
        
        var success = await _bookingService.DisputeBookingAsync(bookingId, reason);
        
        if (success)
        {
            TempData["SuccessMessage"] = "Le litige a été signalé avec succès.";
            return RedirectToAction("Details", "Booking", new { id = bookingId });
        }
        else
        {
            TempData["ErrorMessage"] = "Une erreur s'est produite lors du signalement du litige.";
            return RedirectToAction("Details", "Booking", new { id = bookingId });
        }
    }
}
