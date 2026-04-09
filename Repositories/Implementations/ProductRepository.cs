using GamblersGrocery.Data;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Repositories.Implementations
{
	public class ProductRepository : IProductRepository
	{
		private readonly AppDbContext _ctx;
		private readonly ILogger<ProductRepository> _logger;

		public ProductRepository(AppDbContext ctx, ILogger<ProductRepository> logger)
		{
			_ctx = ctx;
			_logger = logger;
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			try
			{
				return await _ctx.Products
					.OrderBy(p => p.productName)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetAllProducts failed");
				throw;
			}
		}

		public async Task<Product?> GetProductByIdAsync(int id)
		{
			try
			{
				return await _ctx.Products.FindAsync(id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetProductById failed for {Id}", id);
				throw;
			}
		}

		public async Task<Product?> GetProductByBarcodeAsync(string barcode)
		{
			try
			{
				return await _ctx.Products.FirstOrDefaultAsync(p => p.barcode == barcode);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetProductByBarcode failed");
				throw;
			}
		}

		public async Task AddProductAsync(Product product)
		{
			try
			{
				_ctx.Products.Add(product);
				await _ctx.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AddProduct failed");
				throw;
			}
		}

		public async Task UpdateProductAsync(Product product)
		{
			try
			{
				_ctx.Products.Update(product);
				await _ctx.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "UpdateProduct failed");
				throw;
			}
		}

		public async Task DeleteProductAsync(int id)
		{
			try
			{
				var product = await _ctx.Products.FindAsync(id);

				if (product != null)
				{
					_ctx.Products.Remove(product);
					await _ctx.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "DeleteProduct failed for {Id}", id);
				throw;
			}
		}

		public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5)
		{
			try
			{
				return await _ctx.Products
					.Where(p => p.stockQuantity <= threshold)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetLowStock failed");
				throw;
			}
		}
	}
}