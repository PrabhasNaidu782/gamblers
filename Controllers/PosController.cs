using System.Text.Json;
using GamblersGrocery.Data;
using GamblersGrocery.Filters;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Models.ViewModels;
using GamblersGrocery.Repositories.Implementations;
using GamblersGrocery.Repositories.Interfaces;
using GamblersGrocery.Services.Implementations;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Controllers
{
    [SessionAuthorize("Admin", "Cashier")]
    public class PosController : Controller
    {
        private const string BillKey = "CurrentBill";

        private readonly IPosService _posService;
        private readonly ILogger<PosController> _logger;
        private readonly IProductService _productService;

        public PosController(IPosService posService, IProductService productService, ILogger<PosController> logger)
        {
            _posService = posService;
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.AllProducts = await _posService.GetCurrentStockLevelsAsync();
                return View("Billing", GetBill());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load Billing index");
                return View("Billing", new BillViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ScanProduct(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                TempData["Error"] = "Please enter a barcode.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var product = await _posService.ScanProductAsync(barcode.Trim());
                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction(nameof(Index));
                }

                var bill = GetBill();
                var existing = bill.Items.FirstOrDefault(i => i.productId == product.productId);

                if (existing != null)
                {
                    int currentQty = existing.quantity ?? 0;

                    // VALIDATION: Check against actual stock only
                    if (currentQty + 1 > product.stockQuantity)
                    {
                        TempData["Error"] = $"Cannot add more. Only {product.stockQuantity} in stock.";
                        return RedirectToAction(nameof(Index));
                    }

                    existing.quantity++;
                }
                else
                {
                    // NEW ITEM: Ensure we have at least 1 in stock before adding
                    if (product.stockQuantity < 1)
                    {
                        TempData["Error"] = "Product is out of stock.";
                        return RedirectToAction(nameof(Index));
                    }

                    bill.Items.Add(new BillItemViewModel
                    {
                        productId = product.productId,
                        productName = product.productName,
                        barcode = product.barcode,
                        unitPrice = product.price,
                        quantity = 1,
                        availableStock = product.stockQuantity ?? 0
                    });
                }

                bill = await _posService.ApplyDiscountAsync(bill, bill.billDiscountPercent);
                SaveBill(bill);

                TempData["Success"] = $"'{product.productName}' added.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ScanProduct failed");
                TempData["Error"] = "An error occurred while scanning.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UpdateQty(int productId, int? quantity)
        {
            var bill = GetBill();
            var item = bill.Items.FirstOrDefault(i => i.productId == productId);
            var product = await _productService.GetProductDetailsAsync(productId);

            if (item != null && product != null && quantity.HasValue)
            {
                if (quantity > product.stockQuantity)
                {
                    
                    item.quantity = product.stockQuantity;

                    
                    TempData["Error"] = $"Max quantity for  {product.productName} is in stock ({product.stockQuantity}).";
                }
                else if (quantity < 1)
                {
                    item.quantity = 1;
                }
                else
                {
                    item.quantity = quantity;
                }
            }

            bill = await _posService.ApplyDiscountAsync(bill, bill.billDiscountPercent);
            SaveBill(bill);

            
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(decimal discountPercent)
        {
            try
            {
                var bill = GetBill();
                bill = await _posService.ApplyDiscountAsync(bill, discountPercent);
                SaveBill(bill);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ApplyDiscount failed");
                TempData["Error"] = "Could not apply discount.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string paymentMode)
        {
            var bill = GetBill();
            if (!bill.Items.Any())
            {
                TempData["Error"] = "Bill is empty.";
                return RedirectToAction(nameof(Index));
            }

            if (paymentMode == "CASH")
            {
                return await CompleteSale(paymentMode, null);
            }

            return RedirectToAction(nameof(PaymentGateway), new { mode = paymentMode });
        }

        [HttpGet]
        public IActionResult PaymentGateway(string mode)
        {
            var bill = GetBill();
            if (!bill.Items.Any()) return RedirectToAction(nameof(Index));

            ViewBag.PaymentMode = mode;
            return View("PaymentGateway", bill);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteSale(string paymentMode, string? upiId)
        {
            var bill = GetBill();
            if (!bill.Items.Any())
            {
                TempData["Error"] = "Bill is empty.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                string cashierName = SessionHelper.GetUserName(HttpContext.Session);
                int cashierId = SessionHelper.GetUserId(HttpContext.Session);

                bill.paymentMode = paymentMode;
                bill = await _posService.ApplyDiscountAsync(bill, bill.billDiscountPercent);

                var tx = await _posService.CompleteSaleAsync(bill, cashierId, cashierName, paymentMode, upiId);

                HttpContext.Session.Remove(BillKey);

                TempData["Success"] = $"Sale completed via {paymentMode}! Bill #{tx.transactionId}";
                return RedirectToAction(nameof(GetTransactionDetails), new { transactionId = tx.transactionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CompleteSale failed");
                TempData["Error"] = "Could not complete sale.";
                return RedirectToAction(nameof(Index));
            }
        }

        [SessionAuthorize("Admin", "Cashier", "Store Manager")]
        public async Task<IActionResult> GetTransactionDetails(int transactionId)
        {
            try
            {
                var tx = await _posService.GetTransactionDetailsAsync(transactionId);
                if (tx == null) return NotFound();
                return View("TransactionDetails", tx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTransactionDetails failed");
                TempData["Error"] = "Could not load transaction.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> RemoveItem(int productId)
        {
            try
            {
                var bill = GetBill();
                bill.Items.RemoveAll(i => i.productId == productId);
                bill = await _posService.ApplyDiscountAsync(bill, bill.billDiscountPercent);
                SaveBill(bill);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveItem failed");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult ClearBill()
        {
            HttpContext.Session.Remove(BillKey);
            TempData["Success"] = "Bill cleared.";
            return RedirectToAction(nameof(Index));
        }

        private BillViewModel GetBill()
        {
            var json = HttpContext.Session.GetString(BillKey);
            if (string.IsNullOrEmpty(json)) return new BillViewModel();
            try
            {
                return JsonSerializer.Deserialize<BillViewModel>(json) ?? new BillViewModel();
            }
            catch
            {
                return new BillViewModel();
            }
        }

        private void SaveBill(BillViewModel bill)
            => HttpContext.Session.SetString(BillKey, JsonSerializer.Serialize(bill));
    }
}