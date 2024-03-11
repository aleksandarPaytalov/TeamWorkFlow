using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskService _service;

        public TaskController(ITaskService service)
        {
            _service = service;
        }

        public async Task<IActionResult> All()
        {
            var model = await _service.GetAllTasksAsync();

            return View();
        }
    }
}
