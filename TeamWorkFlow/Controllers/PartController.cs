using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    public class PartController : BaseController
    {
        public IActionResult All()
        {
            return View();
        }
    }
}
