using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebUI.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult ImageList()
        {
            return View();
        }
    }
}
