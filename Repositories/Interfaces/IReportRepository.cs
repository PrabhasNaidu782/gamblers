using GamblersGrocery.Models.Entities;
namespace GamblersGrocery.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Settlement>> GetAllSettlementsAsync();
        Task<Settlement?> GetSettlementByIdAsync(int id);
        Task AddSettlementAsync(Settlement settlement);
        Task<decimal> GetTotalSalesByDateAsync(DateTime date);
    }
}
