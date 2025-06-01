using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.ViewModels.Admin;
using Pawesome.Services;

namespace Pawesome.Controllers
{
    /// <summary>
    /// Controller responsible for handling administrative operations
    /// Only accessible by users with Admin role
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAdvertService _advertService;
        private readonly IBookingService _bookingService;
        private readonly IReportService _reportService;
        private readonly IPetService _petService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="userService">Service for user management operations</param>
        /// <param name="advertService">Service for advert management operations</param>
        /// <param name="bookingService">Service for booking management operations</param>
        /// <param name="petService">Service for pet management operations</param>
        public AdminController(
            IUserService userService,
            IAdvertService advertService,
            IBookingService bookingService,
            IReportService reportService,
            IPetService petService)
        {
            _userService = userService;
            _advertService = advertService;
            _bookingService = bookingService;
            _reportService = reportService;
            _petService = petService;
        }

        /// <summary>
        /// Displays the admin dashboard with overview statistics
        /// </summary>
        /// <returns>Admin dashboard view with statistics</returns>
        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new AdminDashboardViewModel
            {
                UsersCount = _userService.GetUsersCount(),
                AdvertsCount = await _advertService.GetAdvertsCountAsync(),
                BookingsCount = _bookingService.GetBookingsCount(),
                PetsCount = await _petService.GetPetsCountAsync(),
                ReportsCount = await _reportService.GetReportsCountAsync()
            };

            return View(dashboardViewModel);
        }

        /// <summary>
        /// Displays the users management page
        /// </summary>
        /// <returns>Users management view</returns>
        public IActionResult Users()
        {
            return View();
        }

        /// <summary>
        /// Gets all users for the admin dashboard
        /// </summary>
        /// <returns>JSON formatted user data for DataTables</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Json(new { data = users });
        }

        /// <summary>
        /// Displays the adverts management page
        /// </summary>
        /// <returns>Adverts management view</returns>
        public IActionResult Adverts()
        {
            return View();
        }

        /// <summary>
        /// Gets all adverts for the admin dashboard
        /// </summary>
        /// <returns>JSON formatted advert data for DataTables</returns>
        [HttpGet]
        public IActionResult GetAdverts()
        {
            var adverts = _advertService.GetAllAdverts();
            return Json(new { data = adverts });
        }
        
        /// <summary>
        /// Bans a user from the platform
        /// </summary>
        /// <param name="id">ID of the user to ban</param>
        /// <returns>JSON result indicating success or failure</returns>
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
    
        /// <summary>
        /// Unbans a previously banned user
        /// </summary>
        /// <param name="id">ID of the user to unban</param>
        /// <returns>JSON result indicating success or failure</returns>
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
        
        /// <summary>
        /// Displays the reports management page for administrators.
        /// </summary>
        /// <returns>The reports management view.</returns>
        public IActionResult Reports()
        {
            return View();
        }

        /// <summary>
        /// Displays the reports management page.
        /// </summary>
        /// <returns>Reports management view.</returns>
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _reportService.GetAllReportsAsync();
            var userService = HttpContext.RequestServices.GetRequiredService<IUserService>();
    
            var enrichedReports = new List<object>();
    
            foreach (var report in reports)
            {
                var targetUser = await userService.GetUserProfileAsync(report.TargetId);
        
                enrichedReports.Add(new {
                    id = report.Id,
                    targetId = report.TargetId,
                    targetName = targetUser?.FirstName + ' ' + targetUser?.LastName,
                    user = report.User != null ? new {
                        id = report.User.Id,
                        email = report.User.Email,
                        displayName = report.User.FirstName + ' ' + report.User.LastName
                    } : null,
                    createdAt = report.CreatedAt,
                    comment = report.Comment,
                    isResolved = report.IsResolved,
                    status = report.Status
                });
            }
    
            return Json(new { data = enrichedReports });
        }
        
        /// <summary>
        /// Marks a report as resolved.
        /// </summary>
        /// <param name="id">The ID of the report to resolve.</param>
        /// <returns>
        /// A JSON result indicating whether the operation was successful or not.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> ResolveReport(int id)
        {
            try
            {
                await _reportService.ResolveReportAsync(id);
                return Json(new { success = true, message = "Signalement marqué comme résolu" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
