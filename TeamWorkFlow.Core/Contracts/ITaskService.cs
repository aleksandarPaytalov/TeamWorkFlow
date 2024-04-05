using TeamWorkFlow.Core.Models.Task;

namespace TeamWorkFlow.Core.Contracts
{
    public interface ITaskService
    {
        Task<ICollection<TaskServiceModel>> GetAllTasksAsync();
        Task<ICollection<TaskStatusServiceModel>> GetAllStatusesAsync();
        Task<ICollection<TaskPriorityServiceModel>> GetAllPrioritiesAsync();
        Task AddNewTaskAsync(AddTaskFormModel model, string userId);
        Task<bool> TaskStatusExistsAsync(int statusId);
        Task<bool> PriorityExistsAsync(int priorityId);

    }
}
