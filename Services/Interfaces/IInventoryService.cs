using GamblersGrocery.Models.Entities;
namespace GamblersGrocery.Services.Interfaces
{
    public interface IInventoryService
    {
        Task UpdateStockAsync(int productId, int qty, string changeType);
        Task AdjustStockAsync(int productId, int newQuantity);
        Task<IEnumerable<StockLog>> GetStockLogsAsync();
        Task<IEnumerable<Product>> GetCurrentStockLevelsAsync();
    }
}
