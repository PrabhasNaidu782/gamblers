using GamblersGrocery.Data;

namespace GamblersGrocery.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task AddUserAsync(AppUser user);
        Task DeleteUserAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task<int> CountAdminsAsync();
    }
}