using GamblersGrocery.Data;
using GamblersGrocery.Filters;
using GamblersGrocery.Models.ViewModels;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GamblersGrocery.Controllers
{
    [SessionAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IReportService reportService,
            IUserService userService,
            ILogger<AdminController> logger)
        {
            _reportService = reportService;
            _userService = userService;
            _logger = logger;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                return View(await _reportService.GetAdminDashboardDataAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard failed");
                TempData["Error"] = "Could not load dashboard.";
                return View(new AdminDashboardViewModel());
            }
        }

        // GET: /Admin/ManageUsers
        public async Task<IActionResult> ManageUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return View(users.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ManageUsers failed");
                TempData["Error"] = "Could not load users.";
                return View(new List<AppUser>());
            }
        }

        // GET: /Admin/CreateUser
        public IActionResult CreateUser() => View(new RegisterViewModel());

        // POST: /Admin/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            try
            {
                bool created = await _userService.CreateUserAsync(
                    vm.FullName, vm.Email, vm.Password, vm.Role);

                if (!created)
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(vm);
                }

                TempData["Success"] = $"User '{vm.FullName}' created as {vm.Role}.";
                return RedirectToAction(nameof(ManageUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUser failed");
                ModelState.AddModelError("", "An error occurred.");
                return View(vm);
            }
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                int currentUserId = SessionHelper.GetUserId(HttpContext.Session);

                bool deleted = await _userService.DeleteUserAsync(userId, currentUserId);

                if (!deleted)
                {
                    TempData["Error"] = "Cannot delete — either the only Admin or your own account.";
                    return RedirectToAction(nameof(ManageUsers));
                }

                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction(nameof(ManageUsers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUser failed");
                TempData["Error"] = "Could not delete user.";
                return RedirectToAction(nameof(ManageUsers));
            }
        }
    }
}