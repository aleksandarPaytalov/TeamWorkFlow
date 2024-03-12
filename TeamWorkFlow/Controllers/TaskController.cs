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

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await _service.GetAllTasksAsync();

            return View(model);
        }

        public IActionResult Details()
        {
	        return View();
        }

        public IActionResult Edit()
        {
	        return View();
        }

        public IActionResult Add()
        {
	        return View();
        }

        public IActionResult Delete()
        {
	        return View();
        }
	}
}
