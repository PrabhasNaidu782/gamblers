using GamblersGrocery.Models.Entities;
using GamblersGrocery.Models.ViewModels;
namespace GamblersGrocery.Services.Interfaces
{
    public interface IPosService
    {
        Task<Product?> ScanProductAsync(string barcode);
        Task<BillViewModel> ApplyDiscountAsync(BillViewModel bill, decimal extraDiscountPercent);
        Task<Transaction> CompleteSaleAsync(BillViewModel bill, int cashierId, string cashierName, string paymentMode, string? upiId = null);
        Task<Transaction?> GetTransactionDetailsAsync(int id);
        Task<IEnumerable<Product>> GetCurrentStockLevelsAsync();

    }
}
