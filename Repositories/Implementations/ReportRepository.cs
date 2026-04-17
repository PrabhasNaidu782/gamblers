using GamblersGrocery.Data;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<ReportRepository> _logger;
        public ReportRepository(AppDbContext ctx, ILogger<ReportRepository> logger) { _ctx = ctx; _logger = logger; }

        public async Task<IEnumerable<Settlement>> GetAllSettlementsAsync()
        {
            try { return await _ctx.Settlements.OrderBy(s => s.settlementDate).ToListAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "GetAllSettlements failed"); throw; }
        }
        public async Task<Settlement?> GetSettlementByIdAsync(int id)
        {
            try { return await _ctx.Settlements.FindAsync(id); }
            catch (Exception ex) { _logger.LogError(ex, "GetSettlementById failed"); throw; }
        }
        public async Task AddSettlementAsync(Settlement settlement)
        {
            try { _ctx.Settlements.Add(settlement); await _ctx.SaveChangesAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "AddSettlement failed"); throw; }
        }
        public async Task<decimal> GetTotalSalesByDateAsync(DateTime date)
        {
            try { return await _ctx.Transactions.Where(t => t.transactionDate.Date == date.Date).SumAsync(t => (decimal?)t.finalAmount) ?? 0; }
            catch (Exception ex) { _logger.LogError(ex, "GetTotalSalesByDate failed"); throw; }
        }
    }
}
