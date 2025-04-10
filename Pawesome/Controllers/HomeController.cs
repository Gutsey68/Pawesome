using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Models;

namespace Pawesome.Controllers;

/// <summary>
/// Controller handling the main pages of the application
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the HomeController
    /// </summary>
    /// <param name="logger">The logger for diagnostic information</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the home page of the application
    /// </summary>
    /// <returns>The index view</returns>
    public IActionResult Index()
    {
        var cardList = new List<PetCartLandingViewModel>
        {
            new PetCartLandingViewModel
            {
                ImageUrl = "/images/landing/cat.png",
                AnimalName = "Pumpkin",
                AnimalType = "Chat",
                TagColor = "#FFEFD6",
                City = "Paris",
                Country = "France",
                Description = "Petit chat gentil, mordille pour jouer mais pas agressif"
            },
            
            new PetCartLandingViewModel
            {
            ImageUrl = "/images/landing/rabbit.png",
            AnimalName = "Crakut",
            AnimalType = "Lapin",
            TagColor = "#E4FFE3",
            City = "Strasbourg",
            Country = "France",
            Description = "Petit lapin gentil, mordille pour jouer mais pas agressif"
            },
            
            new PetCartLandingViewModel
            {
            ImageUrl = "/images/landing/dog.png",
            AnimalName = "Pumpkin",
            AnimalType = "Chien",
            TagColor = "#E6F4FE",
            City = "Paris",
            Country = "France",
            Description = "Chien joueur et super affectueux !"
            }
        };

        return View(cardList);
    }

    /// <summary>
    /// Displays the privacy policy page
    /// </summary>
    /// <returns>The privacy view</returns>
    public IActionResult Privacy()
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