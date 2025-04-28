using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Models;
using Pawesome.Models.ViewModels;

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
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("", "dashboard");
        }
        
        var model = new List<PetCartLandingViewModel>
        {
            new PetCartLandingViewModel
            {
                ImageLink = "/images/landing/cat.png",
                Name = "Mozzarella",
                Species = "Chat",
                TagColor = "#FFEFD6",
                City = "Lyon",
                Country = "France",
                Description = "A tenté de hacker la litière connectée. Ronronne en binaire."
            },

            new PetCartLandingViewModel
            { 
                ImageLink = "/images/landing/rabbit.png",
                Name = "Jean-Lapin",
                Species = "Lapin",
                TagColor = "#E4FFE3",
                City = "Bordeaux",
                Country = "France",
                Description = "Aime le jazz et les carottes bio. Tape du pied quand il kiffe."
            },

            new PetCartLandingViewModel
            {
                ImageLink = "/images/landing/dog.png",
                Name = "Biscotte",
                Species = "Chien",
                TagColor = "#E6F4FE",
                City = "Nice",
                Country = "France",
                Description = "Professionnel en câlins. Ronfle plus fort que ton voisin du dessus."
            },

            new PetCartLandingViewModel
            {
                ImageLink = "/images/landing/dog2.png",
                Name = "Pixel",
                Species = "Chien",
                TagColor = "#FFE8D8",
                City = "Marseille",
                Country = "France",
                Description = "Joueur invétéré. Connaît plus de tricks que ton assistant vocal."
            },

            new PetCartLandingViewModel
            {
                ImageLink = "/images/landing/cat2.png",
                Name = "Lady Miaouscar",
                Species = "Chat",
                TagColor = "#defffd",
                City = "Lille",
                Country = "France",
                Description = "Critique gastronomique de croquettes. Très exigeante, très fluffy."
            }

        };
        
        return View(model);
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