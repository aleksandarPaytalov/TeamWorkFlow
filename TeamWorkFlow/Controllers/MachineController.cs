using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    public class MachineController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
