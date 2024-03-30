using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    public class ProjectController : BaseController
    {
        public IActionResult All()
        {
            return View();
        }
    }
}
