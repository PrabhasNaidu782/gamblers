using GamblersGrocery.Models.Entities;
namespace GamblersGrocery.Services.Interfaces
{
    public interface IProductService
    {
        IEnumerable<string> GetCategories();
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductDetailsAsync(int id);
        Task<Product?> GetProductByBarcodeAsync(string barcode);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
    }
}
