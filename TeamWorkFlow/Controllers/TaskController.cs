using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Task;

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

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var priorities = await _service.GetAllPrioritiesAsync();
            var statuses = await _service.GetAllStatusesAsync();

            var model = new AddTaskViewModel()
            {
	            Statuses = statuses,
	            Priorities = priorities
            };

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

        

        public IActionResult Delete()
        {
	        return View();
        }
	}
}
