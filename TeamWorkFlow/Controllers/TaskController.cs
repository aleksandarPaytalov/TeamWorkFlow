using Microsoft.AspNetCore.Mvc;

namespace TeamWorkFlow.Controllers
{
    public class TaskController : BaseController
    {
        public IActionResult All()
        {
            return View();
        }
    }
}
