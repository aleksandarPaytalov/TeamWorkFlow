using TeamWorkFlow.Core.Models.Task;

namespace TeamWorkFlow.Core.Contracts
{
    public interface ITaskService
    {
        Task<ICollection<TaskViewModel>> GetAllTasksAsync();
        Task<ICollection<StatusViewModel>> GetAllStatusesAsync();
        Task<ICollection<PriorityViewModel>> GetAllPrioritiesAsync();
        Task AddNewTaskAsync(AddTaskViewModel model, string userId);
        Task<bool> TaskStatusExistsAsync(int statusId);
        Task<bool> PriorityExistsAsync(int priorityId);

    }
}
