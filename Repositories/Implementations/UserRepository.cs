using GamblersGrocery.Data;
using GamblersGrocery.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GamblersGrocery.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext ctx, ILogger<UserRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _ctx.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex) { _logger.LogError(ex, "GetUserByEmail failed"); throw; }
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            try
            {
                return await _ctx.AppUsers
                    .OrderBy(u => u.FullName)
                    .ToListAsync();
            }
            catch (Exception ex) { _logger.LogError(ex, "GetAllUsers failed"); throw; }
        }

        public async Task AddUserAsync(AppUser user)
        {
            try { _ctx.AppUsers.Add(user); await _ctx.SaveChangesAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "AddUser failed"); throw; }
        }

        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _ctx.AppUsers.FindAsync(userId);
                if (user != null)
                {
                    _ctx.AppUsers.Remove(user);
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "DeleteUser failed"); throw; }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try { return await _ctx.AppUsers.AnyAsync(u => u.Email == email); }
            catch (Exception ex) { _logger.LogError(ex, "EmailExists failed"); throw; }
        }

        public async Task<int> CountAdminsAsync()
        {
            try { return await _ctx.AppUsers.CountAsync(u => u.Role == "Admin"); }
            catch (Exception ex) { _logger.LogError(ex, "CountAdmins failed"); throw; }
        }
    }
}