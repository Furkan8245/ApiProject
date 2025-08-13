using Microsoft.AspNetCore.Mvc;

namespace ApiProject.WebUI.ViewComponents
{
    public class _FeatureDefaultComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
