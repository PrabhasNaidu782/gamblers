using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using GamblersGrocery.Services.Interfaces;

namespace GamblersGrocery.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _repo;
        private readonly ILogger<PromotionService> _logger;

        public PromotionService(IPromotionRepository repo, ILogger<PromotionService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<Promotion>> GetPromotionsAsync()
        {
            try
            {
                return await _repo.GetAllPromotionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PromotionService GetAll failed");
                throw;
            }
        }

        public async Task<Promotion?> GetPromotionByIdAsync(int id)
        {
            try
            {
                return await _repo.GetPromotionByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PromotionService GetById failed");
                throw;
            }
        }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            try
            {
                return await _repo.GetActivePromotionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PromotionService GetActive failed");
                throw;
            }
        }

        public async Task AddPromotionAsync(Promotion promotion)
        {
            try
            {
                await _repo.AddPromotionAsync(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PromotionService Add failed");
                throw;
            }
        }

        public async Task UpdatePromotionAsync(Promotion promotion)
        {
            try
            {
                await _repo.UpdatePromotionAsync(promotion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PromotionService Update failed");
                throw;
            }
        }

        public async Task DeletePromotionAsync(int id)
        {
            try
            {
                await _repo.DeletePromotionAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PromotionService Delete failed");
                throw;
            }
        }
    }
}
