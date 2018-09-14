using Microsoft.AspNetCore.Mvc;

namespace AnyPreview.Portal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Running");
        }
    }
}