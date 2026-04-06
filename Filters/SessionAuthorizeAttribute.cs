using GamblersGrocery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GamblersGrocery.Filters
{
    // Replaces [Authorize] - checks session instead of cookie
    // Usage: [SessionAuthorize]  or  [SessionAuthorize("Admin","Cashier")]
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public SessionAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // Not logged in
            if (!SessionHelper.IsLoggedIn(session))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Role check
            if (_roles.Length > 0 && !SessionHelper.IsInRoles(session, _roles))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
