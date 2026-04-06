using GamblersGrocery.Data;
using GamblersGrocery.Filters;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Models.ViewModels;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GamblersGrocery.Controllers
{
    [SessionAuthorize("Admin", "Store Manager", "Cashier")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [SessionAuthorize("Admin", "Store Manager")]
        public async Task<IActionResult> Index()
        {
            try { return View(await _reportService.GetAllSettlementsAsync()); }
            catch (Exception ex) { _logger.LogError(ex, "Report Index failed"); TempData["Error"] = "Could not load settlements."; return View(Enumerable.Empty<Settlement>()); }
        }

        [SessionAuthorize("Admin", "Store Manager")]
        public async Task<IActionResult> GenerateDailySalesReport(DateTime? date = null)
        {
            try { return View(await _reportService.GenerateDailySalesReportAsync(date?.Date ?? DateTime.Today)); }
            catch (Exception ex) { _logger.LogError(ex, "GenerateReport failed"); TempData["Error"] = "Could not generate report."; return RedirectToAction(nameof(Index)); }
        }

        public async Task<IActionResult> CloseDay()
        {
            try
            {
                // Get cashier name from session - no Identity needed
                var cashierName = SessionHelper.GetUserName(HttpContext.Session);
                var report = await _reportService.GenerateDailySalesReportAsync(DateTime.Today);
                return View(new CloseDayViewModel { date = DateTime.Today, systemTotal = report.totalSales, cashierName = cashierName });
            }
            catch (Exception ex) { _logger.LogError(ex, "CloseDay GET failed"); TempData["Error"] = "Could not load data."; return RedirectToAction(nameof(Index)); }
        }

        [HttpPost]
        public async Task<IActionResult> CloseDayPost(CloseDayViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var report = await _reportService.GenerateDailySalesReportAsync(DateTime.Today);
                vm.systemTotal = report.totalSales;
                return View("CloseDay", vm);
            }
            try
            {
                var cashierName = SessionHelper.GetUserName(HttpContext.Session);
                var cashierId = SessionHelper.GetUserId(HttpContext.Session);
                var settlement = await _reportService.CloseDayAsync(cashierId, cashierName, vm.cashInHand, vm.date);
                TempData["Success"] = $"Day closed. Variance: Rs {settlement.variance:N2}";
                return RedirectToAction(nameof(GetSettlementDetails), new { settlementId = settlement.settlementId });
            }
            catch (Exception ex) { _logger.LogError(ex, "CloseDayPost failed"); TempData["Error"] = "An error occurred."; return View("CloseDay", vm); }
        }

        [SessionAuthorize("Admin", "Store Manager")]
        public async Task<IActionResult> GetSettlementDetails(int settlementId)
        {
            try { var s = await _reportService.GetSettlementDetailsAsync(settlementId); if (s == null) return NotFound(); return View("SettlementDetails", s); }
            catch (Exception ex) { _logger.LogError(ex, "GetSettlementDetails failed"); TempData["Error"] = "Could not load details."; return RedirectToAction(nameof(Index)); }
        }
    }
}
