using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Sprint;
using static TeamWorkFlow.Core.Constants.Messages;

namespace TeamWorkFlow.Controllers
{
    [Authorize]
    public class SprintController : Controller
    {
        private readonly ISprintService sprintService;

        public SprintController(ISprintService sprintService)
        {
            this.sprintService = sprintService;
        }

        /// <summary>
        /// Sprint To Do main page - GitHub RoadMap style
        /// </summary>
        public async Task<IActionResult> Index(
            string searchTerm = "",
            string statusFilter = "",
            string priorityFilter = "",
            string projectFilter = "",
            string operatorFilter = "",
            string machineFilter = "",
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                var viewModel = await sprintService.GetSprintPlanningDataAsync(
                    searchTerm, statusFilter, priorityFilter, projectFilter,
                    operatorFilter, machineFilter, page, pageSize);

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData[UserMessageError] = "Error loading sprint data. Please try again.";
                return View(new SprintPlanningViewModel());
            }
        }

        /// <summary>
        /// Add task to sprint via AJAX
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTaskToSprint(int taskId, int sprintOrder)
        {
            try
            {
                var (canAdd, reason) = await sprintService.ValidateTaskForSprintAsync(taskId);
                if (!canAdd)
                {
                    return Json(new { success = false, message = reason });
                }

                var success = await sprintService.AddTaskToSprintAsync(taskId, sprintOrder);
                if (success)
                {
                    return Json(new { success = true, message = "Task added to sprint successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add task to sprint" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while adding the task" });
            }
        }

        /// <summary>
        /// Remove task from sprint via AJAX
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTaskFromSprint(int taskId)
        {
            try
            {
                var success = await sprintService.RemoveTaskFromSprintAsync(taskId);
                if (success)
                {
                    return Json(new { success = true, message = "Task removed from sprint successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to remove task from sprint" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while removing the task" });
            }
        }

        /// <summary>
        /// Update sprint task order via drag and drop
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTaskOrder([FromBody] Dictionary<int, int> taskOrders)
        {
            try
            {
                var success = await sprintService.UpdateSprintTaskOrderAsync(taskOrders);
                if (success)
                {
                    return Json(new { success = true, message = "Task order updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update task order" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating task order" });
            }
        }

        /// <summary>
        /// Update task estimated time
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEstimatedTime(int taskId, int estimatedTime)
        {
            try
            {
                if (estimatedTime <= 0)
                {
                    return Json(new { success = false, message = "Estimated time must be greater than 0" });
                }

                var success = await sprintService.UpdateTaskEstimatedTimeAsync(taskId, estimatedTime);
                if (success)
                {
                    return Json(new { success = true, message = "Estimated time updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update estimated time" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating estimated time" });
            }
        }

        /// <summary>
        /// Update task planned dates
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePlannedDates(int taskId, DateTime? plannedStartDate, DateTime? plannedEndDate)
        {
            try
            {
                if (plannedStartDate.HasValue && plannedEndDate.HasValue && plannedStartDate > plannedEndDate)
                {
                    return Json(new { success = false, message = "Start date cannot be after end date" });
                }

                var success = await sprintService.UpdateTaskPlannedDatesAsync(taskId, plannedStartDate, plannedEndDate);
                if (success)
                {
                    return Json(new { success = true, message = "Planned dates updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update planned dates" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while updating planned dates" });
            }
        }

        /// <summary>
        /// Get capacity data for AJAX updates
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCapacityData()
        {
            try
            {
                var capacity = await sprintService.GetSprintCapacityAsync();
                return Json(new { success = true, data = capacity });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to load capacity data" });
            }
        }

        /// <summary>
        /// Get sprint summary for dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSprintSummary()
        {
            try
            {
                var summary = await sprintService.GetSprintSummaryAsync();
                return Json(new { success = true, data = summary });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to load sprint summary" });
            }
        }

        /// <summary>
        /// Clear all tasks from sprint
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearSprint()
        {
            try
            {
                var success = await sprintService.ClearSprintAsync();
                if (success)
                {
                    TempData[UserMessageSuccess] = "Sprint cleared successfully";
                    return Json(new { success = true, message = "Sprint cleared successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to clear sprint" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred while clearing sprint" });
            }
        }

        /// <summary>
        /// Auto-assign tasks to sprint based on priority and capacity
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoAssignTasks(int maxTasks = 10)
        {
            try
            {
                var assignedCount = await sprintService.AutoAssignTasksToSprintAsync(maxTasks);

                if (assignedCount > 0)
                {
                    TempData[UserMessageSuccess] = $"{assignedCount} tasks auto-assigned to sprint";
                    return Json(new { success = true, message = $"{assignedCount} tasks assigned successfully", count = assignedCount });
                }
                else
                {
                    return Json(new { success = false, message = "No tasks could be assigned (capacity or validation constraints)", count = 0 });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred during auto-assignment" });
            }
        }
    }
}
