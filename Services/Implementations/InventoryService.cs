using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using GamblersGrocery.Services.Interfaces;

namespace GamblersGrocery.Services.Implementations
{
	public class InventoryService : IInventoryService
	{
		private readonly IInventoryRepository _repo;
		private readonly IProductRepository _productRepo;
		private readonly ILogger<InventoryService> _logger;

		public InventoryService(
			IInventoryRepository repo,
			IProductRepository productRepo,
			ILogger<InventoryService> logger)
		{
			_repo = repo;
			_productRepo = productRepo;
			_logger = logger;
		}

		public async Task UpdateStockAsync(int productId, int qty, string changeType)
		{
			try
			{
				await _repo.UpdateStockAsync(productId, qty, changeType);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "InventoryService UpdateStock failed");
				throw;
			}
		}

		public async Task AdjustStockAsync(int productId, int newQuantity)
		{
			try
			{
				var product = await _productRepo.GetProductByIdAsync(productId);

				if (product == null)
				{
					return;
				}

				int diff = (int)(newQuantity - product.stockQuantity);
				product.stockQuantity = newQuantity;

				await _productRepo.UpdateProductAsync(product);

				await _repo.AddStockLogAsync(new StockLog
				{
					productId = productId,
					quantityChanged = diff,
					changeType = "MANUAL_ADJUSTMENT",
					timestamp = DateTime.Now
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "InventoryService AdjustStock failed");
				throw;
			}
		}

		public async Task<IEnumerable<StockLog>> GetStockLogsAsync()
		{
			try
			{
				return await _repo.GetAllStockLogsAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "InventoryService GetLogs failed");
				throw;
			}
		}

		public async Task<IEnumerable<Product>> GetCurrentStockLevelsAsync()
		{
			try
			{
				return await _productRepo.GetAllProductsAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "InventoryService GetLevels failed");
				throw;
			}
		}
	}
}