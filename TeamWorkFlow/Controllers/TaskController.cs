using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Extensions;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Extensions;
using static TeamWorkFlow.Core.Constants.Messages;
using static TeamWorkFlow.Constants.MessageConstants;

namespace TeamWorkFlow.Controllers
{
    public class TaskController : BaseController
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;

        public TaskController(ITaskService taskService, 
	        IProjectService projectService)
        {
	        _taskService = taskService;
	        _projectService = projectService;
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
	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

			var priorities = await _taskService.GetAllPrioritiesAsync();
            var statuses = await _taskService.GetAllStatusesAsync();

            var model = new TaskFormModel()
            {
	            Statuses = statuses,
	            Priorities = priorities
            };

	        return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskFormModel model)
        {
            var userId = User.Id();

            DateTime? parsedEndDate = null;
            DateTime? parsedDeadlineDate = null;

            // Check if the startDate is in valid format.
            bool startDateIsValid = DateTime.TryParseExact(model.StartDate, DateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate);
            if (!startDateIsValid)
            {
                ModelState.AddModelError(nameof(model.StartDate), string.Format(InvalidDate, DateFormat));
            }

            // If the EndDate is not null and valid it checks if the StartDate is smaller than the EndDate.
            if (!string.IsNullOrWhiteSpace(model.EndDate))
            {
                bool endDateIsValid = DateTime.TryParseExact(model.EndDate, DateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);
                if (!endDateIsValid)
                {
                    ModelState.AddModelError(nameof(model.EndDate), string.Format(InvalidDate, DateFormat));
                }
                else
                {
                    if (parsedStartDate > endDate)
                    {
                        ModelState.AddModelError(nameof(model.EndDate), string.Format(StartDateGreaterThanEndDateOrDeadLine));
                    }
                    else
                    {
                        parsedEndDate = endDate;
                    }
                }
            }

            // If the Deadline is not null and valid it checks if the StartDate is smaller than the Deadline.
            if (!string.IsNullOrWhiteSpace(model.Deadline))
            {
                bool deadLineIsValid = DateTime.TryParseExact(model.Deadline, Messages.DateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deadLine);
                if (!deadLineIsValid)
                {
                    ModelState.AddModelError(nameof(model.Deadline), string.Format(InvalidDate, DateFormat));
                }
                else
                {
                    if (parsedStartDate > deadLine)
                    {
                        ModelState.AddModelError(nameof(model.Deadline), string.Format(StartDateGreaterThanEndDateOrDeadLine));
                    }
                    else
                    {
                        parsedDeadlineDate = deadLine;
                    }
                }
            }

            if (await _taskService.PriorityExistsAsync(model.PriorityId) == false)
            {
                ModelState.AddModelError(nameof(model.PriorityId), $"{PriorityNotExisting}");
            }

            if (await _taskService.TaskStatusExistsAsync(model.StatusId) == false)
            {
                ModelState.AddModelError(nameof(model.PriorityId), $"{StatusNotExisting}");
            }

            if (!await _projectService.ExistByProjectNumberAsync(model.ProjectNumber))
            {
                ModelState.AddModelError(nameof(model.ProjectNumber), string.Format(ProjectWithGivenNumberDoNotExist));
            }

            int validProjectId = 0;
            int? projectId = await _projectService.GetProjectIdByProjectNumberAsync(model.ProjectNumber);
            if (projectId.HasValue && projectId.Value != 0)
            {
                validProjectId = projectId.Value;
            }

            if (ModelState.IsValid == false)
            {
                model.Priorities = await _taskService.GetAllPrioritiesAsync();
                model.Statuses = await _taskService.GetAllStatusesAsync();

                TempData["UserMessageError"] = $"{InValidDataInsert}";

                return View(model);
            }

            int taskId = await _taskService.AddNewTaskAsync(model, userId, parsedStartDate, parsedEndDate, parsedDeadlineDate, validProjectId);

            TempData["UserMessageSuccess"] = $"{ValidDataInsert}";

            return RedirectToAction(nameof(Details), new { id = taskId, extension = model.GetTaskExtension() });
        }


        [HttpGet]
		public async Task <IActionResult> Details(int id, string extension)
        {
	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

			if (!await _taskService.TaskExistByIdAsync(id))
	        {
		        return BadRequest();
	        }

	        var taskModel = await _taskService.GetTaskDetailsByIdAsync(id);

            if (extension != taskModel?.GetTaskExtension())
            {
                return BadRequest();
            }

	        return View(taskModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string extension)
        {
	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

			if (!await _taskService.TaskExistByIdAsync(id))
	        {
		        return BadRequest();
	        }

	        var taskModel = await _taskService.GetTaskForEditByIdAsync(id);

	        if (extension != taskModel?.GetTaskExtension())
	        {
		        return BadRequest();
	        }

			return View(taskModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskFormModel model, int id)
        {
	        if (!await _taskService.TaskExistByIdAsync(id))
	        {
		        return BadRequest();
	        }

	        DateTime? parsedEndDate = null;
	        DateTime? parsedDeadlineDate = null;

			//Check if the startDate is in valid format.
			bool startDateIsValid = DateTime.TryParseExact(model.StartDate, DateFormat,
		        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedStartDate);
			if (!startDateIsValid)
	        {
		        ModelState.AddModelError(nameof(model.StartDate), string.Format(InvalidDate, DateFormat));
	        }

			//If the EndDate is not null and valid it checks if the StartDate is smaller than the EndDate.
	        if (!string.IsNullOrWhiteSpace(model.EndDate))
	        {
		        bool endDateIsValid = DateTime.TryParseExact(model.EndDate, DateFormat,
			        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate);
		        if (!endDateIsValid)
		        {
			        ModelState.AddModelError(nameof(model.EndDate), string.Format(InvalidDate, DateFormat));
				}
		        else
		        {
			        if (parsedStartDate > endDate)
			        {
						ModelState.AddModelError(nameof(model.EndDate), string.Format(StartDateGreaterThanEndDateOrDeadLine));
			        }
			        else
			        {
				        parsedEndDate = endDate;
			        }
		        }
			}
	        
			//If the Deadline is not null and valid it checks if the StartDate is smaller than the Deadline.
			if (!string.IsNullOrWhiteSpace(model.Deadline))
	        {
		        bool deadLineIsValid = DateTime.TryParseExact(model.Deadline, Messages.DateFormat,
			        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deadLine);
		        if (!deadLineIsValid)
		        {
			        ModelState.AddModelError(nameof(model.Deadline), string.Format(InvalidDate, DateFormat));
				}
		        else
		        {
			        if (parsedStartDate > deadLine)
			        {
				        ModelState.AddModelError(nameof(model.Deadline), string.Format(StartDateGreaterThanEndDateOrDeadLine));
					}
			        else
			        {
				        parsedDeadlineDate = deadLine;
			        }
		        }
			}
			
			if (await _taskService.PriorityExistsAsync(model.PriorityId) == false)
	        {
		        ModelState.AddModelError(nameof(model.PriorityId), $"{PriorityNotExisting}");
	        }

	        if (await _taskService.TaskStatusExistsAsync(model.StatusId) == false)
	        {
		        ModelState.AddModelError(nameof(model.PriorityId), $"{StatusNotExisting}");
	        }

	        if (!await _projectService.ExistByProjectNumberAsync(model.ProjectNumber))
	        {
				ModelState.AddModelError(nameof(model.ProjectNumber), string.Format(ProjectWithGivenNumberDoNotExist));
	        }

	        int validProjectId = 0;
			int? projectId = await _projectService.GetProjectIdByProjectNumberAsync(model.ProjectNumber);
			if (projectId.HasValue && projectId.Value != 0)
			{
				validProjectId = projectId.Value;
			}
			
			if (ModelState.IsValid == false)
	        {
		        model.Priorities = await _taskService.GetAllPrioritiesAsync();
		        model.Statuses = await _taskService.GetAllStatusesAsync();

		        return View(model);
	        }
	        

	        await _taskService.EditTaskAsync(model, id, parsedStartDate, parsedEndDate, parsedDeadlineDate, validProjectId);

			return RedirectToAction(nameof(Details), new {id, extension = model.GetTaskExtension()});
		}

		[HttpGet]
        public async Task<IActionResult> Delete(int id, string extension)
        {
	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

			if (!await _taskService.TaskExistByIdAsync(id))
			{
				return BadRequest();
			}

			var model = await _taskService.GetTaskForDeleteByIdAsync(id);

			if (extension != model?.GetTaskExtension())
			{
				return BadRequest();
			}

	        return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
	        if (!await _taskService.TaskExistByIdAsync(id))
	        {
		        return BadRequest();
	        }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _taskService.DeleteTaskAsync(id);

			return RedirectToAction(nameof(All));
		}

		[HttpGet]
        public async Task<IActionResult> AddToMine(int id)
        {
	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

			var taskToAdd = await _taskService.GetTaskByIdAsync(id);

	        if (taskToAdd == null)
	        {
		        return RedirectToAction(nameof(All));
			}

	        var userId = User.Id();

	        await _taskService.AddTaskToMyCollection(taskToAdd, userId);

	        return RedirectToAction(nameof(Mine));
        }

		[HttpGet]
        public async Task<IActionResult> Mine()
        {
	        var userId = User.Id();

	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

	        var model = await _taskService.GetMyTasksAsync(userId);
			
			return View(model);
        }

        public async Task<IActionResult> RemoveFromCollection(int id)
        {
	        var userId = User.Id();

	        if (User.IsAdmin() == false && User.IsOperator() == false)
	        {
		        return Unauthorized();
	        }

	        await _taskService.RemoveFromCollection(id, userId);

			return RedirectToAction(nameof(Mine));
        }
	}
}
