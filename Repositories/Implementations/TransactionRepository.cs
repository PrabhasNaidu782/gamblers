using GamblersGrocery.Data;
using GamblersGrocery.Models.Entities;
using GamblersGrocery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Repositories.Implementations
{
	public class TransactionRepository : ITransactionRepository
	{
		private readonly AppDbContext _ctx;
		private readonly ILogger<TransactionRepository> _logger;

		public TransactionRepository(AppDbContext ctx, ILogger<TransactionRepository> logger)
		{
			_ctx = ctx;
			_logger = logger;
		}

		public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
		{
			try
			{
				return await _ctx.Transactions
					.Include(t => t.TransactionItems)
						.ThenInclude(ti => ti.Product)
					.OrderByDescending(t => t.transactionDate)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetAllTransactions failed");
				throw;
			}
		}

		public async Task<Transaction?> GetTransactionByIdAsync(int id)
		{
			try
			{
				return await _ctx.Transactions
					.Include(t => t.TransactionItems)
						.ThenInclude(ti => ti.Product)
					.FirstOrDefaultAsync(t => t.transactionId == id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetTransactionById failed");
				throw;
			}
		}

		public async Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(DateTime date)
		{
			try
			{
				return await _ctx.Transactions
					.Include(t => t.TransactionItems)
						.ThenInclude(ti => ti.Product)
					.Where(t => t.transactionDate.Date == date.Date)
					.OrderByDescending(t => t.transactionDate)
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetTransactionsByDate failed");
				throw;
			}
		}

		public async Task AddTransactionAsync(Transaction transaction)
		{
			try
			{
				_ctx.Transactions.Add(transaction);
				await _ctx.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AddTransaction failed");
				throw;
			}
		}
	}
}