using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    public class OperatorController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
