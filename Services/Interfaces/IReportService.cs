using GamblersGrocery.Models.Entities;
using GamblersGrocery.Models.ViewModels;
namespace GamblersGrocery.Services.Interfaces
{
    public interface IReportService
    {
        Task<DailySalesReportViewModel> GenerateDailySalesReportAsync(DateTime date);
        Task<Settlement> CloseDayAsync(int cashierId, string cashierName, decimal cashInHand, DateTime date);
        Task<Settlement?> GetSettlementDetailsAsync(int id);
        Task<IEnumerable<Settlement>> GetAllSettlementsAsync();
        Task<AdminDashboardViewModel> GetAdminDashboardDataAsync();
    }
}
