using GamblersGrocery.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GamblersGrocery.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
