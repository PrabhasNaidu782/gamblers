using System.ComponentModel;
using GamblersGrocery.Filters;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GamblersGrocery.Controllers
{
    [SessionAuthorize("Admin", "Store Manager")]
//new comment another
    public class PromotionController : Controller
    {
        
        private readonly IPromotionService _promoService;
        private readonly IProductService _productService;
        private readonly ILogger<PromotionController> _logger;
        public PromotionController(IPromotionService promoService, IProductService productService, ILogger<PromotionController> logger) { _promoService = promoService; _productService = productService; _logger = logger; }

        public async Task<IActionResult> Index()
        {
            try { return View(await _promoService.GetPromotionsAsync()); }
            catch (Exception ex) { _logger.LogError(ex, "Promotion Index failed"); TempData["Error"] = "Could not load promotions."; return View(Enumerable.Empty<Promotion>()); }
        }

        public async Task<IActionResult> Create()
        {
            var products = await _productService.GetAllProductsAsync();
            var categories = products.Select(p => p.category).Distinct().OrderBy(c => c).ToList();
            ViewBag.Categories = new SelectList(categories);
            ViewBag.ProductList = products;

            return View(new Promotion { startDate = DateTime.Today, endDate = DateTime.Today.AddDays(7) });
        }

        [HttpPost]
        public async Task<IActionResult> AddPromotion(Promotion promotion)
        {
            if (promotion.endDate < promotion.startDate) ModelState.AddModelError("endDate", "End date must be after start date.");
            if (!ModelState.IsValid) { var products = await _productService.GetAllProductsAsync(); ViewBag.Products = new SelectList(products, "productId", "productName", promotion.productId); return View("Create", promotion); }
            try { await _promoService.AddPromotionAsync(promotion); TempData["Success"] = "Promotion added!"; return RedirectToAction(nameof(Index)); }
            catch (Exception ex) { _logger.LogError(ex, "AddPromotion failed"); ModelState.AddModelError("", "An error occurred."); var products = await _productService.GetAllProductsAsync(); ViewBag.Products = new SelectList(products, "productId", "productName", promotion.productId); return View("Create", promotion); }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try { var promo = await _promoService.GetPromotionByIdAsync(id); if (promo == null) return NotFound(); var products = await _productService.GetAllProductsAsync(); ViewBag.Products = new SelectList(products, "productId", "productName", promo.productId); return View(promo); }
            catch (Exception ex) { _logger.LogError(ex, "Promotion Edit failed"); TempData["Error"] = "Could not load promotion."; return RedirectToAction(nameof(Index)); }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePromotion(Promotion promotion)
        {
            if (promotion.endDate < promotion.startDate) ModelState.AddModelError("endDate", "End date must be after start date.");
            if (!ModelState.IsValid) { var products = await _productService.GetAllProductsAsync(); ViewBag.Products = new SelectList(products, "productId", "productName", promotion.productId); return View("Edit", promotion); }
            try { await _promoService.UpdatePromotionAsync(promotion); TempData["Success"] = "Promotion updated!"; return RedirectToAction(nameof(Index)); }
            catch (Exception ex) { _logger.LogError(ex, "UpdatePromotion failed"); ModelState.AddModelError("", "An error occurred."); var products = await _productService.GetAllProductsAsync(); ViewBag.Products = new SelectList(products, "productId", "productName", promotion.productId); return View("Edit", promotion); }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try { var promo = await _promoService.GetPromotionByIdAsync(id); if (promo == null) return NotFound(); return View(promo); }
            catch (Exception ex) { _logger.LogError(ex, "Promotion Delete failed"); TempData["Error"] = "Could not load promotion."; return RedirectToAction(nameof(Index)); }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int promotionId)
        {
            try { 
                await _promoService.DeletePromotionAsync(promotionId); TempData["Success"] = "Promotion deleted!"; return RedirectToAction(nameof(Index)); }
            catch (Exception ex) { _logger.LogError(ex, "DeleteConfirmed failed"); TempData["Error"] = "Could not delete."; return RedirectToAction(nameof(Index)); }
        }
    }
}
