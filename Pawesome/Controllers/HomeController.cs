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
        
        var model = new List<PetCartViewModel>
        {
            new PetCartViewModel
            {
                Photo = "/images/landing/cat.png",
                Name = "Mozzarella",
                Species = "Chat",
                City = "Lyon",
                Country = "France",
                Info = "A tenté de hacker la litière connectée. Ronronne en binaire."
            },

            new PetCartViewModel
            { 
                Photo = "/images/landing/rabbit.png",
                Name = "Jean-Lapin",
                Species = "Rongeur",
                City = "Bordeaux",
                Country = "France",
                Info = "Aime le jazz et les carottes bio. Tape du pied quand il kiffe."
            },

            new PetCartViewModel
            {
                Photo = "/images/landing/dog.png",
                Name = "Biscotte",
                Species = "Chien",
                City = "Nice",
                Country = "France",
                Info = "Professionnel en câlins. Ronfle plus fort que ton voisin du dessus."
            },

            new PetCartViewModel
            {
                Photo = "/images/landing/dog2.png",
                Name = "Pixel",
                Species = "Chien",
                City = "Marseille",
                Country = "France",
                Info = "Joueur invétéré. Connaît plus de tricks que ton assistant vocal."
            },

            new PetCartViewModel
            {
                Photo = "/images/landing/cat2.png",
                Name = "Lady Miaouscar",
                Species = "Chat",
                City = "Lille",
                Country = "France",
                Info = "Critique gastronomique de croquettes. Très exigeante, très fluffy."
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
    /// Displays the terms and conditions (CGV) page.
    /// </summary>
    /// <returns>The CGV view.</returns>
    public IActionResult Cgv()
    {
        return View();
    }
    
    [Route("Home/HandleError/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        var viewModel = new ErrorViewModel
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        };

        return View(statusCode == 404 ? "NotFound" : "Error", viewModel);
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