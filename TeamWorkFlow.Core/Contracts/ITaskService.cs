using TeamWorkFlow.Core.Models.Task;

namespace TeamWorkFlow.Core.Contracts
{
    public interface ITaskService
    {
        Task<ICollection<TaskViewModel>> GetAllTasksAsync();
        Task<ICollection<StatusViewModel>> GetAllStatusesAsync();
        Task<ICollection<PriorityViewModel>> GetAllPrioritiesAsync();
        Task AddNewTaskAsync(AddTaskViewModel model, string userId);

    }
}
