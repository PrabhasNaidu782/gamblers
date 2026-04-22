using GamblersGrocery.Models.Entities;
using GamblersGrocery.Models.ViewModels;
using GamblersGrocery.Repositories.Interfaces;
using GamblersGrocery.Services.Interfaces;

namespace GamblersGrocery.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepo;
        private readonly ITransactionRepository _txRepo;
        private readonly IProductRepository _productRepo;
        private readonly IPromotionRepository _promoRepo;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IReportRepository reportRepo,
            ITransactionRepository txRepo,
            IProductRepository productRepo,
            IPromotionRepository promoRepo,
            ILogger<ReportService> logger)
        {
            _reportRepo = reportRepo;
            _txRepo = txRepo;
            _productRepo = productRepo;
            _promoRepo = promoRepo;
            _logger = logger;
        }

        public async Task<DailySalesReportViewModel> GenerateDailySalesReportAsync(DateTime date)
        {
            try
            {
                var list = (await _txRepo.GetTransactionsByDateAsync(date)).ToList();

                return new DailySalesReportViewModel
                {
                    reportDate = date,
                    Transactions = list,
                    totalTransactions = list.Count,
                    totalSales = list.Sum(t => t.finalAmount),
                    totalDiscount = list.Sum(t => t.discountAmount),
                    PaymentBreakdown = list
                        .GroupBy(t => t.paymentMode)
                        .ToDictionary(g => g.Key, g => g.Count())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportService GenerateReport failed");
                throw;
            }
        }

        public async Task<Settlement> CloseDayAsync(int cashierId, string cashierName, decimal cashInHand, DateTime date)
        {
            try
            {
                var systemTotal = await _reportRepo.GetTotalSalesByDateAsync(date);

                var settlement = new Settlement
                {
                    cashierId = cashierId,
                    cashierName = cashierName,
                    totalSales = systemTotal,
                    cashInHand = cashInHand,
                    variance = cashInHand - systemTotal,
                    settlementDate = date
                };

                await _reportRepo.AddSettlementAsync(settlement);
                return settlement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportService CloseDay failed");
                throw;
            }
        }

        public async Task<Settlement?> GetSettlementDetailsAsync(int id)
        {
            try
            {
                return await _reportRepo.GetSettlementByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportService GetSettlement failed");
                throw;
            }
        }

        public async Task<IEnumerable<Settlement>> GetAllSettlementsAsync()
        {
            try
            {
                return await _reportRepo.GetAllSettlementsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportService GetAllSettlements failed");
                throw;
            }
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync()
        {
            try
            {
                var today = DateTime.Today;
                var todayTx = (await _txRepo.GetTransactionsByDateAsync(today)).ToList();
                var allProducts = (await _productRepo.GetAllProductsAsync()).ToList();
                var lowStock = (await _productRepo.GetLowStockProductsAsync()).ToList();
                var activePromos = (await _promoRepo.GetActivePromotionsAsync()).ToList();
                var allTx = (await _txRepo.GetAllTransactionsAsync()).ToList();

                return new AdminDashboardViewModel
                {
                    TodaySales = todayTx.Sum(t => t.finalAmount),
                    TodayTransactions = todayTx.Count,
                    TotalProducts = allProducts.Count,
                    LowStockCount = lowStock.Count,
                    ActivePromotions = activePromos.Count,
                    LowStockProducts = lowStock,
                    RecentTransactions = allTx.Take(10).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportService Dashboard failed");
                throw;
            }
        }
    }
}
