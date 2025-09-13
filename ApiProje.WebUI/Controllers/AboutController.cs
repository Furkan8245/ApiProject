using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebUI.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
