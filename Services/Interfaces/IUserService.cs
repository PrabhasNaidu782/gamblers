using GamblersGrocery.Data;

namespace GamblersGrocery.Services.Interfaces
{
    public interface IUserService
    {
        Task<AppUser?> ValidateLoginAsync(string email, string password);
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(string fullName, string email, string password, string role);
        Task<bool> DeleteUserAsync(int userId, int currentUserId);
    }
}