using TeamWorkFlow.Core.Models.Task;

namespace TeamWorkFlow.Core.Contracts
{
    public interface ITaskService
    {
        Task<ICollection<TaskServiceModel>> GetAllTasksAsync();
        Task<ICollection<TaskStatusServiceModel>> GetAllStatusesAsync();
        Task<ICollection<TaskPriorityServiceModel>> GetAllPrioritiesAsync();
        Task AddNewTaskAsync(TaskFormModel model, 
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

    }
}
