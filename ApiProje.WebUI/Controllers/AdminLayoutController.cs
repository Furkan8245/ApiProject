using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebUI.Controllers
{
    public class AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
