namespace GamblersGrocery.Data
{
    // Keys used to store user info in session
    public static class SessionKeys
    {
        public const string UserId   = "SessionUserId";
        public const string UserName = "SessionUserName";
        public const string UserEmail= "SessionUserEmail";
        public const string UserRole = "SessionUserRole";
    }

    // Helpers to read session values
    public static class SessionHelper
    {
        public static bool IsLoggedIn(ISession session)
            => !string.IsNullOrEmpty(session.GetString(SessionKeys.UserId));

        public static string GetUserName(ISession session)
            => session.GetString(SessionKeys.UserName) ?? "";

        public static string GetUserEmail(ISession session)
            => session.GetString(SessionKeys.UserEmail) ?? "";

        public static string GetUserRole(ISession session)
            => session.GetString(SessionKeys.UserRole) ?? "";

        public static int GetUserId(ISession session)
            => int.TryParse(session.GetString(SessionKeys.UserId), out var id) ? id : 0;

        public static bool IsInRole(ISession session, string role)
            => string.Equals(GetUserRole(session), role, StringComparison.OrdinalIgnoreCase);

        public static bool IsInRoles(ISession session, params string[] roles)
            => roles.Any(r => IsInRole(session, r));

        public static void SetUser(ISession session, AppUser user)
        {
            session.SetString(SessionKeys.UserId,    user.UserId.ToString());
            session.SetString(SessionKeys.UserName,  user.FullName);
            session.SetString(SessionKeys.UserEmail, user.Email);
            session.SetString(SessionKeys.UserRole,  user.Role);
        }

        public static void Clear(ISession session)
        {
            session.Remove(SessionKeys.UserId);
            session.Remove(SessionKeys.UserName);
            session.Remove(SessionKeys.UserEmail);
            session.Remove(SessionKeys.UserRole);
        }
    }
}
