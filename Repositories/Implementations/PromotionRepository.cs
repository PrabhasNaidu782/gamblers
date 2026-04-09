using GamblersGrocery.Data;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Repositories.Implementations
{
	public class PromotionRepository : IPromotionRepository
	{
		private readonly AppDbContext _ctx;
		private readonly ILogger<PromotionRepository> _logger;

		public PromotionRepository(AppDbContext ctx, ILogger<PromotionRepository> logger)
		{
			_ctx = ctx;
			_logger = logger;
		}

		public async Task<IEnumerable<Promotion>> GetAllPromotionsAsync()
		{
			try
			{
				return await _ctx.Promotions
					.Include(p => p.Product)
					.OrderByDescending(p => p.promotionId)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetAllPromotions failed");
				throw;
			}
		}

		public async Task<Promotion?> GetPromotionByIdAsync(int id)
		{
			try
			{
				return await _ctx.Promotions
					.Include(p => p.Product)
					.FirstOrDefaultAsync(p => p.promotionId == id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetPromotionById failed");
				throw;
			}
		}

		public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
		{
			try
			{
				var today = DateTime.Today;
				return await _ctx.Promotions
					.Include(p => p.Product)
					.Where(p => p.startDate <= today && p.endDate >= today)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetActivePromotions failed");
				throw;
			}
		}

		public async Task<Promotion?> GetActivePromotionForProductAsync(int productId)
		{
			try
			{
				var today = DateTime.Today;
				return await _ctx.Promotions
					.Where(p => p.productId == productId && p.startDate <= today && p.endDate >= today)
					.OrderByDescending(p => p.discountPercent)
					.FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetActivePromotionForProduct failed");
				throw;
			}
		}

		public async Task AddPromotionAsync(Promotion promotion)
		{
			try
			{
				_ctx.Promotions.Add(promotion);
				await _ctx.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AddPromotion failed");
				throw;
			}
		}

		public async Task UpdatePromotionAsync(Promotion promotion)
		{
			try
			{
				_ctx.Promotions.Update(promotion);
				await _ctx.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "UpdatePromotion failed");
				throw;
			}
		}

		public async Task DeletePromotionAsync(int id)
		{
			try
			{
				var promotion = await _ctx.Promotions.FindAsync(id);

				if (promotion != null)
				{
					_ctx.Promotions.Remove(promotion);
					await _ctx.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "DeletePromotion failed");
				throw;
			}
		}
	}
}