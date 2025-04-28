using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Models;
using Pawesome.Models.ViewModels;

namespace Pawesome.Controllers;

/// <summary>
/// Controller handling the main pages of the application
/// </summary>
public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    /// <summary>
    /// Initializes a new instance of the HomeController
    /// </summary>
    /// <param name="logger">The logger for diagnostic information</param>
    public DashboardController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the home page of the application
    /// </summary>
    /// <returns>The index view</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Handles error display with diagnostic information
    /// </summary>
    /// <returns>The error view with request identification details</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}