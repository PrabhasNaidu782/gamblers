using GamblersGrocery.Filters;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GamblersGrocery.Controllers
{
    [SessionAuthorize("Admin", "Store Manager")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger) { _productService = productService; _logger = logger; }

        public async Task<IActionResult> Index()
        {
            try { return View(await _productService.GetAllProductsAsync()); }
            catch (Exception ex) { _logger.LogError(ex, "Product Index failed"); TempData["Error"] = "Could not load products."; return View(Enumerable.Empty<Product>()); }
        }

        public async Task<IActionResult> GetProductDetails(int id)
        {
            try { var p = await _productService.GetProductDetailsAsync(id); if (p == null) return NotFound(); return View("Details", p); }
            catch (Exception ex) { _logger.LogError(ex, "GetProductDetails failed"); TempData["Error"] = "Could not load product."; return RedirectToAction(nameof(Index)); }
        }

        public IActionResult Create()
        { ViewBag.Categories = new SelectList(_productService.GetCategories()); return View(new Product()); }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (!ModelState.IsValid) { ViewBag.Categories = new SelectList(_productService.GetCategories()); return View("Create", product); }
            try { await _productService.AddProductAsync(product); TempData["Success"] = $"Product '{product.productName}' added!"; return RedirectToAction(nameof(Index)); }
            catch (Exception ex) { _logger.LogError(ex, "AddProduct failed"); ModelState.AddModelError("", "An error occurred."); ViewBag.Categories = new SelectList(_productService.GetCategories()); return View("Create", product); }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try { var p = await _productService.GetProductDetailsAsync(id); if (p == null) return NotFound(); ViewBag.Categories = new SelectList(_productService.GetCategories(), p.category); return View(p); }
            catch (Exception ex) { _logger.LogError(ex, "Edit GET failed"); TempData["Error"] = "Could not load product."; return RedirectToAction(nameof(Index)); }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if (!ModelState.IsValid) { ViewBag.Categories = new SelectList(_productService.GetCategories()); return View("Edit", product); }
            try { await _productService.UpdateProductAsync(product); TempData["Success"] = "Product updated!"; return RedirectToAction(nameof(Index)); }
            catch (Exception ex) { _logger.LogError(ex, "UpdateProduct failed"); ModelState.AddModelError("", "An error occurred."); ViewBag.Categories = new SelectList(_productService.GetCategories()); return View("Edit", product); }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try { var p = await _productService.GetProductDetailsAsync(id); if (p == null) return NotFound(); return View(p); }
            catch (Exception ex) { _logger.LogError(ex, "Delete GET failed"); TempData["Error"] = "Could not load product."; return RedirectToAction(nameof(Index)); }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int productId)
        {
            try { await _productService.DeleteProductAsync(productId); TempData["Success"] = "Product deleted!"; return RedirectToAction(nameof(Index)); }
            catch (Exception ex) { _logger.LogError(ex, "DeleteConfirmed failed"); TempData["Error"] = "Cannot delete — may be linked to transactions."; return RedirectToAction(nameof(Index)); }
        }

        [SessionAuthorize("Admin", "Store Manager", "Cashier", "Inventory Associate")]
        public async Task<IActionResult> ListAllProducts()
        {
            try { return View(await _productService.GetAllProductsAsync()); }
            catch (Exception ex) { _logger.LogError(ex, "ListAll failed"); return View(Enumerable.Empty<Product>()); }
        }

    }
}
