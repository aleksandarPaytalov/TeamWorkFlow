using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    public class PartController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
