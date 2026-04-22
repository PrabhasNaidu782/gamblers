using GamblersGrocery.Data;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<InventoryRepository> _logger;

        public InventoryRepository(AppDbContext ctx, ILogger<InventoryRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<IEnumerable<StockLog>> GetAllStockLogsAsync()
        {
            try
            {
                return await _ctx.StockLogs
                    .Include(s => s.Product)
                    .OrderByDescending(s => s.timestamp)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllStockLogs failed");
                throw;
            }
        }

        public async Task AddStockLogAsync(StockLog log)
        {
            try
            {
                _ctx.StockLogs.Add(log);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddStockLog failed");
                throw;
            }
        }

        public async Task UpdateStockAsync(int productId, int quantityChanged, string changeType)
        {
            try
            {
                var product = await _ctx.Products.FindAsync(productId);
                if (product == null)
                {
                    return;
                }

                product.stockQuantity += quantityChanged;

                if (product.stockQuantity < 0)
                {
                    product.stockQuantity = 0;
                }

                _ctx.Products.Update(product);

                _ctx.StockLogs.Add(new StockLog
                {
                    productId = productId,
                    quantityChanged = quantityChanged,
                    changeType = changeType,
                    timestamp = DateTime.Now
                });

                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateStock failed for {Id}", productId);
                throw;
            }
        }
    }
}
