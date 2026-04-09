using GamblersGrocery.Data;
using GamblersGrocery.Filters;
using GamblersGrocery.Models.ViewModels;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Controllers
{
	[SessionAuthorize("Admin")]
	//new comments

	public class AdminController : Controller
	{
		private readonly IReportService _reportService;
		private readonly AppDbContext _db;
		private readonly ILogger<AdminController> _logger;

		public AdminController(IReportService reportService, AppDbContext db, ILogger<AdminController> logger)
		{
			_reportService = reportService;
			_db = db;
			_logger = logger;
		}

		public async Task<IActionResult> Dashboard()
		{
			try
			{
				var dashboardData = await _reportService.GetAdminDashboardDataAsync();
				return View(dashboardData);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Dashboard failed");
				TempData["Error"] = "Could not load dashboard.";
				return View(new AdminDashboardViewModel());
			}
		}

		public async Task<IActionResult> ManageUsers()
		{
			try
			{
				var users = await _db.AppUsers
					.OrderBy(u => u.FullName)
					.ToListAsync();

				return View(users);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "ManageUsers failed");
				TempData["Error"] = "Could not load users.";
				return View(new List<AppUser>());
			}
		}

		public IActionResult CreateUser()
		{
			return View(new RegisterViewModel());
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser(RegisterViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				return View(vm);
			}

			try
			{
				if (await _db.AppUsers.AnyAsync(u => u.Email == vm.Email))
				{
					ModelState.AddModelError("Email", "Email already exists.");
					return View(vm);
				}

				var newUser = new AppUser
				{
					FullName = vm.FullName,
					Email = vm.Email,
					PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password),
					PlainPassword = vm.Password,
					Role = vm.Role
				};

				_db.AppUsers.Add(newUser);
				await _db.SaveChangesAsync();

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

		[HttpPost]
		public async Task<IActionResult> DeleteUser(int userId)
		{
			try
			{
				var user = await _db.AppUsers.FindAsync(userId);

				if (user != null)
				{
					// Prevent deleting the only admin
					var adminCount = await _db.AppUsers.CountAsync(u => u.Role == "Admin");
					if (user.Role == "Admin" && adminCount <= 1)
					{
						TempData["Error"] = "Cannot delete the only Admin account.";
						return RedirectToAction(nameof(ManageUsers));
					}

					// Prevent deleting yourself
					var currentUserId = SessionHelper.GetUserId(HttpContext.Session);
					if (user.UserId == currentUserId)
					{
						TempData["Error"] = "Cannot delete your own account.";
						return RedirectToAction(nameof(ManageUsers));
					}

					_db.AppUsers.Remove(user);
					await _db.SaveChangesAsync();

					TempData["Success"] = $"User '{user.FullName}' deleted.";
				}

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