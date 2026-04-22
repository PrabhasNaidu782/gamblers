using GamblersGrocery.Data;
using GamblersGrocery.Repositories.Interfaces;
using GamblersGrocery.Services.Interfaces;

namespace GamblersGrocery.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepo, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<AppUser?> ValidateLoginAsync(string email, string password)
        {
            try
            {
                var user = await _userRepo.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return null;
                }

                bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                return valid ? user : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ValidateLogin failed");
                throw;
            }
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepo.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllUsers failed");
                throw;
            }
        }

        public async Task<bool> CreateUserAsync(string fullName, string email, string password, string role)
        {
            try
            {
                if (await _userRepo.EmailExistsAsync(email))
                {
                    return false;
                }

                await _userRepo.AddUserAsync(new AppUser
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    PlainPassword = password,
                    Role = role
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUser failed");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId, int currentUserId)
        {
            try
            {
                if (userId == currentUserId)
                {
                    return false;
                }

                var user = (await _userRepo.GetAllUsersAsync())
                           .FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                {
                    return false;
                }

                if (user.Role == "Admin" && await _userRepo.CountAdminsAsync() <= 1)
                {
                    return false;
                }

                await _userRepo.DeleteUserAsync(userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUser failed");
                throw;
            }
        }
    }
}
