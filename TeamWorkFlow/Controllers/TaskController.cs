using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Task;
using static TeamWorkFlow.Core.Constants.Messages;

namespace TeamWorkFlow.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await _taskService.GetAllTasksAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var priorities = await _taskService.GetAllPrioritiesAsync();
            var statuses = await _taskService.GetAllStatusesAsync();

            var model = new AddTaskFormModel()
            {
	            Statuses = statuses,
	            Priorities = priorities
            };

	        return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTaskFormModel model)
        {
            if (await _taskService.PriorityExistsAsync(model.PriorityId) == false)
            {
                ModelState.AddModelError(nameof(model.PriorityId), $"{PriorityNotExisting}");
            }

            if (await _taskService.TaskStatusExistsAsync(model.StatusId) == false)
            {
                ModelState.AddModelError(nameof(model.PriorityId), $"{StatusNotExisting}");
            }

            var userId = GetUserId();

            if (ModelState.IsValid == false)
            {
                model.Priorities = await _taskService.GetAllPrioritiesAsync();
                model.Statuses = await _taskService.GetAllStatusesAsync();

                return View(model);
            }

            await _taskService.AddNewTaskAsync(model, userId);

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
