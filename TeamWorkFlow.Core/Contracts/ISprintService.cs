using TeamWorkFlow.Core.Models.Sprint;

namespace TeamWorkFlow.Core.Contracts
{
    public interface ISprintService
    {
        /// <summary>
        /// Gets the sprint planning view model with all necessary data
        /// </summary>
        /// <param name="searchTerm">Search term for filtering tasks</param>
        /// <param name="statusFilter">Status filter</param>
        /// <param name="priorityFilter">Priority filter</param>
        /// <param name="projectFilter">Project filter</param>
        /// <param name="operatorFilter">Operator filter</param>
        /// <param name="machineFilter">Machine filter</param>
        /// <param name="page">Current page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Sprint planning view model</returns>
        Task<SprintPlanningViewModel> GetSprintPlanningDataAsync(
            string searchTerm = "",
            string statusFilter = "",
            string priorityFilter = "",
            string projectFilter = "",
            string operatorFilter = "",
            string machineFilter = "",
            int page = 1,
            int pageSize = 20);
        
        /// <summary>
        /// Gets all tasks currently in the sprint
        /// </summary>
        /// <returns>List of sprint tasks</returns>
        Task<List<SprintTaskServiceModel>> GetSprintTasksAsync();
        
        /// <summary>
        /// Gets all tasks in the backlog (not in sprint)
        /// </summary>
        /// <returns>List of backlog tasks</returns>
        Task<List<SprintTaskServiceModel>> GetBacklogTasksAsync();
        
        /// <summary>
        /// Adds a task to the sprint
        /// </summary>
        /// <param name="taskId">Task ID to add</param>
        /// <param name="sprintOrder">Order position in sprint</param>
        /// <returns>Success status</returns>
        Task<bool> AddTaskToSprintAsync(int taskId, int sprintOrder);
        
        /// <summary>
        /// Removes a task from the sprint
        /// </summary>
        /// <param name="taskId">Task ID to remove</param>
        /// <returns>Success status</returns>
        Task<bool> RemoveTaskFromSprintAsync(int taskId);
        
        /// <summary>
        /// Updates the order of tasks in the sprint
        /// </summary>
        /// <param name="taskOrders">Dictionary of task ID to new order</param>
        /// <returns>Success status</returns>
        Task<bool> UpdateSprintTaskOrderAsync(Dictionary<int, int> taskOrders);
        
        /// <summary>
        /// Updates the estimated time for a task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <param name="estimatedTime">New estimated time in hours</param>
        /// <returns>Success status</returns>
        Task<bool> UpdateTaskEstimatedTimeAsync(int taskId, int estimatedTime);
        
        /// <summary>
        /// Updates the planned dates for a task
        /// </summary>
        /// <param name="taskId">Task ID</param>
        /// <param name="plannedStartDate">Planned start date</param>
        /// <param name="plannedEndDate">Planned end date</param>
        /// <returns>Success status</returns>
        Task<bool> UpdateTaskPlannedDatesAsync(int taskId, DateTime? plannedStartDate, DateTime? plannedEndDate);
        
        /// <summary>
        /// Gets capacity analysis for the current sprint
        /// </summary>
        /// <returns>Capacity analysis model</returns>
        Task<SprintCapacityServiceModel> GetSprintCapacityAsync();
        
        /// <summary>
        /// Gets resource availability information
        /// </summary>
        /// <returns>Resource availability model</returns>
        Task<SprintResourceServiceModel> GetResourceAvailabilityAsync();
        
        /// <summary>
        /// Gets sprint timeline data
        /// </summary>
        /// <param name="startDate">Timeline start date</param>
        /// <param name="endDate">Timeline end date</param>
        /// <returns>Timeline data</returns>
        Task<List<SprintTimelineModel>> GetSprintTimelineAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Validates if a task can be added to sprint based on capacity
        /// </summary>
        /// <param name="taskId">Task ID to validate</param>
        /// <returns>Validation result with reason</returns>
        Task<(bool CanAdd, string Reason)> ValidateTaskForSprintAsync(int taskId);
        
        /// <summary>
        /// Gets sprint summary statistics
        /// </summary>
        /// <returns>Sprint summary model</returns>
        Task<SprintSummaryModel> GetSprintSummaryAsync();
        
        /// <summary>
        /// Clears all tasks from the current sprint
        /// </summary>
        /// <returns>Success status</returns>
        Task<bool> ClearSprintAsync();
        
        /// <summary>
        /// Auto-assigns tasks to sprint based on priority and capacity
        /// </summary>
        /// <param name="maxTasks">Maximum number of tasks to assign</param>
        /// <returns>Number of tasks assigned</returns>
        Task<int> AutoAssignTasksToSprintAsync(int maxTasks = 10);
    }
}
