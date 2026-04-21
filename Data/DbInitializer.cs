using GamblersGrocery.Data;

namespace GamblersGrocery.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            const string adminEmail = "admin@gamblers.com";
            const string adminPassword = "Admin@123";
            const string adminName = "Store Admin";
            const string adminRole = "Admin";

            if (!db.AppUsers.Any(u => u.Email == adminEmail))
            {
                db.AppUsers.Add(new AppUser
                {
                    FullName = adminName,
                    Email = adminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    PlainPassword = adminPassword,
                    Role = adminRole
                });
                await db.SaveChangesAsync();
            }
        }
    }
}
