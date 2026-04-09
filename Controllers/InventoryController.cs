using GamblersGrocery.Filters;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GamblersGrocery.Controllers
{
	[SessionAuthorize("Admin", "Store Manager", "Inventory Associate")]
	public class InventoryController : Controller
	{
		private readonly IInventoryService _inventoryService;
		private readonly IProductService _productService;
		private readonly ILogger<InventoryController> _logger;

		public InventoryController(
			IInventoryService inventoryService,
			IProductService productService,
			ILogger<InventoryController> logger)
		{
			_inventoryService = inventoryService;
			_productService = productService;
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
				var stockLevels = await _inventoryService.GetCurrentStockLevelsAsync();
				return View(stockLevels);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Inventory Index failed");
				TempData["Error"] = "Could not load stock.";
				return View(Enumerable.Empty<Product>());
			}
		}

		public async Task<IActionResult> AdjustStock(int productId)
		{
			try
			{
				var product = await _productService.GetProductDetailsAsync(productId);

				if (product == null)
				{
					return NotFound();
				}

				return View(product);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AdjustStock GET failed");
				TempData["Error"] = "Could not load product.";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		public async Task<IActionResult> AdjustStockPost(int productId, int newQuantity)
		{
			if (newQuantity < 0)
			{
				TempData["Error"] = "Quantity cannot be negative.";
				return RedirectToAction(nameof(AdjustStock), new { productId });
			}

			try
			{
				await _inventoryService.AdjustStockAsync(productId, newQuantity);
				TempData["Success"] = "Stock updated!";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AdjustStockPost failed");
				TempData["Error"] = "Could not update stock.";
				return RedirectToAction(nameof(AdjustStock), new { productId });
			}
		}

		public async Task<IActionResult> StockLogs()
		{
			try
			{
				var logs = await _inventoryService.GetStockLogsAsync();
				return View(logs);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "StockLogs failed");
				TempData["Error"] = "Could not load logs.";
				return View(Enumerable.Empty<StockLog>());
			}
		}

		public async Task<IActionResult> GetStockLevels()
		{
			try
			{
				var stockLevels = await _inventoryService.GetCurrentStockLevelsAsync();
				return View("Index", stockLevels);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetStockLevels failed");
				return View("Index", Enumerable.Empty<Product>());
			}
		}
	}
}