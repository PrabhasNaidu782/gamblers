using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using GamblersGrocery.Services.Interfaces;

namespace GamblersGrocery.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repo, ILogger<ProductService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        private static readonly List<string> _categories = new()
        {
            "Fruits & Vegetables",
            "Dairy & Eggs",
            "Beverages",
            "Snacks & Confectionery",
            "Household & Cleaning"
        };

        public IEnumerable<string> GetCategories()
        {
            return _categories;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _repo.GetAllProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService GetAll failed");
                throw;
            }
        }

        public async Task<Product?> GetProductDetailsAsync(int id)
        {
            try
            {
                return await _repo.GetProductByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService GetById failed");
                throw;
            }
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            try
            {
                return await _repo.GetProductByBarcodeAsync(barcode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService GetByBarcode failed");
                throw;
            }
        }

        public async Task AddProductAsync(Product product)
        {
            try
            {
                await _repo.AddProductAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService Add failed");
                throw;
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                await _repo.UpdateProductAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService Update failed");
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            try
            {
                await _repo.DeleteProductAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService Delete failed");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            try
            {
                return await _repo.GetLowStockProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProductService GetLowStock failed");
                throw;
            }
        }
    }
}
