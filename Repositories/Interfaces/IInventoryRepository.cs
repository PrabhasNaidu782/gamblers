using GamblersGrocery.Models.Entities;
namespace GamblersGrocery.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<StockLog>> GetAllStockLogsAsync();
        Task AddStockLogAsync(StockLog log);
        Task UpdateStockAsync(int productId, int quantityChanged, string changeType);
    }
}
