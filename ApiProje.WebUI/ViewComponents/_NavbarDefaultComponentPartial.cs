using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebUI.ViewComponents
{
    public class _NavbarDefaultComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
