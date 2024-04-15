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

		public async Task<IActionResult> RemoveFromCollection(int id)
		{
			var operatorId = await _taskService.GetOperatorIdByAssignedTaskId(id);

			if (operatorId == 0)
			{
				return BadRequest();
			}

			await _taskService.RemoveAssignedTaskFromUserCollection(id, operatorId);


			return RedirectToAction(nameof(AllAssigns), "Task");
		}
	}
}
