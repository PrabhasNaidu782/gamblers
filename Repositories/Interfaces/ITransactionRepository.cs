using GamblersGrocery.Models.Entities;
namespace GamblersGrocery.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetTransactionsByDateAsync(DateTime date);
        Task AddTransactionAsync(Transaction transaction);
    }
}
