using GamblersGrocery.Data;
using GamblersGrocery.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Controllers
{
	public class AccountController : Controller
	{
		private readonly AppDbContext _db;
		private readonly ILogger<AccountController> _logger;

		public AccountController(AppDbContext db, ILogger<AccountController> logger)
		{
			_db = db;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			// Already logged in - redirect
			if (SessionHelper.IsLoggedIn(HttpContext.Session))
				return RedirectToHome();

			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
		{
			if (!ModelState.IsValid) return View(vm);

			try
			{
				// Find user by email
				var user = await _db.AppUsers
					.FirstOrDefaultAsync(u => u.Email == vm.Email);

				// Verify user existence and password hash
				if (user == null || !BCrypt.Net.BCrypt.Verify(vm.Password, user.PasswordHash))
				{
					ModelState.AddModelError("", "Invalid email or password.");
					return View(vm);
				}

				// Store user in session
				SessionHelper.SetUser(HttpContext.Session, user);
				_logger.LogInformation("User {Email} logged in via session", vm.Email);

				// Redirect based on role
				if (user.Role == "Admin")
					return RedirectToAction("Dashboard", "Admin");

				if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
					return Redirect(returnUrl);

				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Login error for {Email}", vm.Email);
				ModelState.AddModelError("", "An error occurred. Please try again.");
				return View(vm);
			}
		}

		[HttpPost]
		public IActionResult Logout()
		{
			SessionHelper.Clear(HttpContext.Session);
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account");
		}

		public IActionResult AccessDenied() => View();

		private IActionResult RedirectToHome()
		{
			var role = SessionHelper.GetUserRole(HttpContext.Session);
			return role == "Admin"
				? RedirectToAction("Dashboard", "Admin")
				: RedirectToAction("Index", "Home");
		}
	}
}