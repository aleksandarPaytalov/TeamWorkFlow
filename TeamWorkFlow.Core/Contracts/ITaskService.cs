﻿using TeamWorkFlow.Core.Models.Task;

namespace TeamWorkFlow.Core.Contracts
{
    public interface ITaskService
    {
        Task<ICollection<TaskServiceModel>> GetAllTasksAsync();
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
        Task<ICollection<TaskServiceModel>> GetAllAssignedTasksAsync();
        Task<int> GetOperatorIdByAssignedTaskId(int taskId);
        Task RemoveAssignedTaskFromUserCollection(int taskId, int operatorId);


    }
}
