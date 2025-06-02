using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Pawesome.Models.Entities;
using Stripe.Reporting;

namespace Pawesome.Controllers;

/// <summary>
/// Controller responsible for handling report-related operations.
/// </summary>
[Authorize]
public class ReportController : Controller
{
    private readonly IReportService _reportService;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportController"/> class.
    /// </summary>
    /// <param name="reportService">The report service.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="userManager">The user manager.</param>
    public ReportController(
        IReportService reportService, 
        IUserService userService,
        UserManager<User> userManager)
    {
        _reportService = reportService;
        _userService = userService;
        _userManager = userManager;
    }
    
    /// <summary>
    /// Displays the report creation form.
    /// </summary>
    /// <param name="targetId">The ID of the reported entity.</param>
    /// <param name="reportType">The type of report.</param>
    /// <returns>The view for creating a new report.</returns>
    [HttpGet]
    public IActionResult Create(int targetId, string reportType)
    {
        var viewModel = new CreateReportViewModel
        {
            TargetId = targetId,
            ReportType = reportType,
            Comment = string.Empty
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processes the submission of a new report.
    /// </summary>
    /// <param name="model">The report creation view model.</param>
    /// <returns>A redirect to the user profile if successful; otherwise, returns the report creation view with errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReportViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ModelState.AddModelError("", "Utilisateur introuvable");
                return View(model);
            }

            var report = new Report
            {
                UserId = user.Id,
                User = user,
                TargetId = model.TargetId,
                ReportType = model.ReportType,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            await _reportService.CreateReportAsync(report);
            TempData["SuccessMessage"] = "Votre signalement a été enregistré avec succès.";

            switch (model.ReportType)
            {
                case "user":
                    TempData["SuccessMessage"] = "Votre signalement de l'utilisateur a été enregistré avec succès.";
                    return RedirectToAction("Profile", "User", new { id = model.TargetId });
                
                case "advert":
                    TempData["SuccessMessage"] = "Votre signalement de l'annonce a été enregistré avec succès.";
                    return RedirectToAction("Details", "Advert", new { id = model.TargetId });
            }

            return View(model);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}
