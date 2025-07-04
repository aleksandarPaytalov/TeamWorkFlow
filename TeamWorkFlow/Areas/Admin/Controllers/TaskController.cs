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

		public async Task<IActionResult> AllAssigns(int page = 1)
		{
			int pageSize = 10; // You can adjust this as needed
			var (tasks, totalCount) = await _taskService.GetAllAssignedTasksAsync(page, pageSize);
			var assignedTasks = new AssignedTasksServiceModel()
			{
				AllAssignedTasks = tasks,
				Pager = new TeamWorkFlow.Core.Models.Pager.PagerServiceModel(totalCount, page, pageSize)
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
