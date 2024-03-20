using System.Security.Claims;
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

        [HttpPost]
        public async Task<IActionResult> Add(AddTaskViewModel model)
        {
            if (await _service.PriorityExistsAsync(model.PriorityId) == false)
            {
                ModelState.AddModelError(nameof(model.PriorityId), "Selected priority does not exist.");
            }

            if (await _service.TaskStatusExistsAsync(model.StatusId) == false)
            {
                ModelState.AddModelError(nameof(model.PriorityId), "Selected status does not exist");
            }
            var userId = GetUserId();

            if (ModelState.IsValid == false)
            {
                model.Priorities = await _service.GetAllPrioritiesAsync();
                model.Statuses = await _service.GetAllStatusesAsync();

                return View(model);
            }

            await _service.AddNewTaskAsync(model, userId);

            return RedirectToAction(nameof(All));

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


        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
	}
}
