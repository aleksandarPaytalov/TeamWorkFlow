using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Core.Models.BulkOperations;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Core.Models.Operator;

namespace TeamWorkFlow.Core.Contracts
{
    public interface ITaskService
    {
        Task<ICollection<TaskServiceModel>> GetAllTasksAsync();
        Task<TaskQueryServiceModel> AllAsync(
            TaskSorting sorting = TaskSorting.LastAdded,
            string? search = null,
            int tasksPerPage = 10,
            int currentPage = 1);
        Task<ICollection<TaskStatusServiceModel>> GetAllStatusesAsync();
        Task<ICollection<TaskPriorityServiceModel>> GetAllPrioritiesAsync();
        Task<int> AddNewTaskAsync(TaskFormModel model, 
	        string userId,
	        DateTime parsedStartDate,
	        DateTime? parsedEndDate,
	        DateTime? parsedDeadline,
	        int projectId);
        Task<bool> TaskStatusExistsAsync(int statusId);
        Task<bool> PriorityExistsAsync(int priorityId);
        Task<TaskDetailsServiceModel?> GetTaskDetailsByIdAsync(int taskId);
        Task<TaskFormModel?> GetTaskForEditByIdAsync(int taskId);
        Task EditTaskAsync(TaskFormModel model , int taskId,
	        DateTime parsedStartDate,
	        DateTime? parsedEndDate,
	        DateTime? parsedDeadline,
	        int projectId);
        Task<TaskDeleteServiceModel?> GetTaskForDeleteByIdAsync(int taskId);
        Task DeleteTaskAsync(int taskId);
        Task<bool> TaskExistByIdAsync(int taskId);


        Task<ICollection<TaskServiceModel>> GetMyTasksAsync(string userId);
        Task AddTaskToMyCollection(TaskServiceModel model, string userId);
        Task<int> GetOperatorIdByUserId(string userId);
        Task<TaskServiceModel?> GetTaskByIdAsync(int id);
        Task<bool> TaskExistInTaskOperatorTableByIdAsync(int taskId);
        Task RemoveFromCollection(int taskId, string userId);
        Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetAllAssignedTasksAsync(int page, int pageSize);
        Task<int> GetOperatorIdByAssignedTaskId(int taskId);
        Task RemoveAssignedTaskFromUserCollection(int taskId, int operatorId);
        Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetAllTasksAsync(int page, int pageSize);

        // Machine assignment methods
        Task<(bool Success, string Message)> AssignMachineToTaskAsync(int taskId, int machineId);
        Task<(bool Success, string Message)> UnassignMachineFromTaskAsync(int taskId);
        Task<ICollection<MachineServiceModel>> GetAvailableMachinesForTaskAsync(int taskId);
        Task<(bool CanAssign, string Reason)> ValidateMachineAssignmentAsync(int taskId, int machineId);

        // Operator assignment methods
        Task<(bool Success, string Message)> AssignOperatorToTaskAsync(int taskId, int operatorId);
        Task<(bool Success, string Message)> UnassignOperatorFromTaskAsync(int taskId, int operatorId);
        Task<ICollection<OperatorServiceModel>> GetAvailableOperatorsForTaskAsync(int taskId);
        Task<ICollection<OperatorServiceModel>> GetAssignedOperatorsForTaskAsync(int taskId);

        // Archive methods
        Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetArchivedTasksAsync(int page, int pageSize);
        Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetArchivedTasksAsync(int page, int pageSize, string? search, TaskSorting sorting);

        // Bulk operations
        Task<BulkOperationResult> BulkDeleteAsync(List<int> taskIds);
        Task<BulkOperationResult> BulkArchiveAsync(List<int> taskIds);

        // Estimated time management
        Task<(bool Success, string Message)> SetEstimatedTimeAsync(int taskId, int estimatedTime);

        // Task status management
        Task<(bool Success, string Message)> ChangeTaskStatusAsync(int taskId, int statusId, string userId);

    }
}
