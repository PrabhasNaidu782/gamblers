using GamblersGrocery.Filters;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GamblersGrocery.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [SessionAuthorize("Admin", "Store Manager")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Product Index failed");
                TempData["Error"] = "Could not load products.";
                return View(Enumerable.Empty<Product>());
            }
        }

        [SessionAuthorize("Admin", "Store Manager")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                var p = await _productService.GetProductDetailsAsync(id);
                if (p == null) return NotFound();

                return View("Details", p);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProductDetails failed");
                TempData["Error"] = "Could not load product.";
                return RedirectToAction(nameof(Index));
            }
        }

        [SessionAuthorize("Store Manager")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_productService.GetCategories());
            return View(new Product());
        }

        [HttpPost]
        [SessionAuthorize("Store Manager")]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_productService.GetCategories());
                return View("Create", product);
            }

            try
            {
                await _productService.AddProductAsync(product);
                TempData["Success"] = $"Product '{product.productName}' added!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddProduct failed");
                ModelState.AddModelError("", "An error occurred.");
                ViewBag.Categories = new SelectList(_productService.GetCategories());
                return View("Create", product);
            }
        }

        [SessionAuthorize("Store Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var p = await _productService.GetProductDetailsAsync(id);
                if (p == null) return NotFound();

                ViewBag.Categories = new SelectList(_productService.GetCategories(), p.category);
                return View(p);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit GET failed");
                TempData["Error"] = "Could not load product.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [SessionAuthorize("Store Manager")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_productService.GetCategories());
                return View("Edit", product);
            }

            try
            {
                await _productService.UpdateProductAsync(product);
                TempData["Success"] = "Product updated!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProduct failed");
                ModelState.AddModelError("", "An error occurred.");
                ViewBag.Categories = new SelectList(_productService.GetCategories());
                return View("Edit", product);
            }
        }

        [SessionAuthorize("Store Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var p = await _productService.GetProductDetailsAsync(id);
                if (p == null) return NotFound();

                return View(p);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete GET failed");
                TempData["Error"] = "Could not load product.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [SessionAuthorize("Store Manager")]
    
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int productId)
        {
            try
            {
                await _productService.DeleteProductAsync(productId);
                TempData["Success"] = "Product deleted successfully.";
            }
            catch (Exception ex)
            {
                // We check the 'InnerException' because that's where the SQL Error 547 lives
                if (ex.ToString().Contains("REFERENCE constraint") || ex.InnerException?.Message.Contains("REFERENCE constraint") == true)
                {
                    // This is the specific message for products with transactions
                    TempData["Error"] = "Cannot delete: This product has historical sales records. Try deactivating it instead.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while trying to delete the product.";
                }

                _logger.LogError(ex, "Delete failed for product {Id}", productId);
            }

            return RedirectToAction(nameof(Index));
        }
        [SessionAuthorize("Admin", "Store Manager")]
        public async Task<IActionResult> ListAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ListAll failed");
                return View(Enumerable.Empty<Product>());
            }
        }
    }
}