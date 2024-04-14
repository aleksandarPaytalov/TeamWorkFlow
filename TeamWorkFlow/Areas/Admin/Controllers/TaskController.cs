using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Admin;

namespace TeamWorkFlow.Areas.Admin.Controllers
{
	public class TaskController : AdminBaseController
	{
		private readonly ITaskService _taskService;

		public TaskController(ITaskService taskService)
		{
			_taskService = taskService;
		}

		public async Task<IActionResult> AllAssigns()
		{
			var assignedTasks = new AssignedTasksServiceModel()
			{
				AllAssignedTasks = await _taskService.GetAllAssignedTasksAsync()
			};

			return View(assignedTasks);
		}

		public async Task<IActionResult> RemoveFromCollection(int id, int operatorId)
		{

			return RedirectToAction(nameof(AllAssigns), "Task");
		}
	}
}
