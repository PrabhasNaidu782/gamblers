using GamblersGrocery.Models.Entities;
namespace GamblersGrocery.Repositories.Interfaces
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetAllPromotionsAsync();
        Task<Promotion?> GetPromotionByIdAsync(int id);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<Promotion?> GetActivePromotionForProductAsync(int productId);
        Task AddPromotionAsync(Promotion promotion);
        Task UpdatePromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(int id);
    }
}
