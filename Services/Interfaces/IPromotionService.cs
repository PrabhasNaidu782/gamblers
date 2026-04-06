using GamblersGrocery.Models.Entities;
namespace GamblersGrocery.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetPromotionsAsync();
        Task<Promotion?> GetPromotionByIdAsync(int id);
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task AddPromotionAsync(Promotion promotion);
        Task UpdatePromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(int id);
    }
}
