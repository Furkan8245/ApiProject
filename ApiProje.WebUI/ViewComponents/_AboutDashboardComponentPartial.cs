using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebUI.ViewComponents
{
    public class _AboutDashboardComponentPartial :ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
