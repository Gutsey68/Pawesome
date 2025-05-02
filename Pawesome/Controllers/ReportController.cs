using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.ViewModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Pawesome.Controllers;

[Authorize]
public class ReportMvcController : Controller
{
    private readonly IReportService _reportService;
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;

    public ReportMvcController(
        IReportService reportService, 
        IUserService userService,
        UserManager<User> userManager)
    {
        _reportService = reportService;
        _userService = userService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult ReportForm(int targetId, string reportType)
    {
        var viewModel = new CreateReportViewModel
        {
            TargetId = targetId,
            ReportType = reportType
        };

        return PartialView("_ReportFormPartial", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReport(CreateReportViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_ReportFormPartial", model);
        }

        try
        {
            // Utilisation de UserManager pour récupérer l'utilisateur actuel
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ModelState.AddModelError("", "Utilisateur introuvable");
                return PartialView("_ReportFormPartial", model);
            }

            // Création du rapport
            var report = new Report
            {
                UserId = user.Id,
                User = user,
                TargetId = model.TargetId,
                ReportType = model.ReportType,
                Comment = model.Comment,
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };

            await _reportService.CreateReportAsync(report);
            return Json(new { success = true });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return PartialView("_ReportFormPartial", model);
        }
    }
}