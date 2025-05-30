using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.ViewModels.Admin;
using Pawesome.Services;

namespace Pawesome.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAdvertService _advertService;
        private readonly IBookingService _bookingService;
        private readonly IPetService _petService;

        public AdminController(
            IUserService userService,
            IAdvertService advertService,
            IBookingService bookingService,
            IPetService petService)
        {
            _userService = userService;
            _advertService = advertService;
            _bookingService = bookingService;
            _petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new AdminDashboardViewModel
            {
                UsersCount = _userService.GetUsersCount(),
                AdvertsCount = _advertService.GetAdvertsCount(),
                BookingsCount = _bookingService.GetBookingsCount(),
                PetsCount = await _petService.GetPetsCountAsync()
            };

            return View(dashboardViewModel);
        }

        public IActionResult Users()
        {
            return View();
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Json(new { data = users });
        }

        public IActionResult Adverts()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAdverts()
        {
            var adverts = _advertService.GetAllAdverts();
            return Json(new { data = adverts });
        }

        public IActionResult Bookings()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetBookings()
        {
            var bookings = _bookingService.GetAllBookings();
            return Json(new { data = bookings });
        }
        
        [HttpPost]
        public async Task<IActionResult> BanUser(int id)
        {
            var result = await _userService.BanUserAsync(id);
        
            if (result)
            {
                return Json(new { success = true, message = "L'utilisateur a été banni avec succès" });
            }
        
            return Json(new { success = false, message = "Impossible de bannir cet utilisateur" });
        }
    
        [HttpPost]
        public async Task<IActionResult> UnbanUser(int id)
        {
            var result = await _userService.UnbanUserAsync(id);
        
            if (result)
            {
                return Json(new { success = true, message = "L'utilisateur a été réactivé avec succès" });
            }
        
            return Json(new { success = false, message = "Impossible de réactiver cet utilisateur" });
        }
    }
}