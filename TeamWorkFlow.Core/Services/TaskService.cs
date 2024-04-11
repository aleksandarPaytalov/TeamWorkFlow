using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Infrastructure.Common;
using TeamWorkFlow.Infrastructure.Data.Models;
using static TeamWorkFlow.Core.Constants.Messages;
using Task = System.Threading.Tasks.Task;
using TaskStatus = TeamWorkFlow.Infrastructure.Data.Models.TaskStatus;

namespace TeamWorkFlow.Core.Services
{
	public class TaskService : ITaskService
    {
        private readonly IRepository _repository;

        public TaskService(IRepository repository)
        {
	        _repository = repository;
        }


        public async Task<ICollection<TaskServiceModel>> GetAllTasksAsync()
        {
            return await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                .Select(t => new TaskServiceModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Status = t.TaskStatus.Name,
                    Priority = t.Priority.Name,
                    ProjectNumber = t.Project.ProjectNumber,
                    StartDate = t.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                    EndDate = t.EndDate != null ? t.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    Deadline = t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty
				})
                .ToListAsync();
        }

        public async Task<ICollection<TaskStatusServiceModel>> GetAllStatusesAsync()
        {
            return await _repository.AllReadOnly<TaskStatus>()
                .Select(s => new TaskStatusServiceModel()
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        public async Task<ICollection<TaskPriorityServiceModel>> GetAllPrioritiesAsync()
        {
            return await _repository.AllReadOnly<Priority>()
                .Select(p => new TaskPriorityServiceModel()
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task<int> AddNewTaskAsync(TaskFormModel model, 
	        string userId,
			DateTime parsedStartDate,
	        DateTime? parsedEndDate,
	        DateTime? parsedDeadline,
	        int projectId)
        {
            var task = new Infrastructure.Data.Models.Task()
            {
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                DeadLine = parsedDeadline,
                CreatorId = userId,
                Description = model.Description,
                Name = model.Name,
                PriorityId = model.PriorityId,
                TaskStatusId = model.StatusId,
                ProjectId = projectId
            };

            await _repository.AddAsync(task);
            await _repository.SaveChangesAsync();

            return task.Id;
        }

        public async Task<bool> TaskStatusExistsAsync(int statusId)
        {
            return await _repository.AllReadOnly<TaskStatus>().AnyAsync(s => s.Id == statusId);
        }

        public async Task<bool> PriorityExistsAsync(int priorityId)
        {
            return await _repository.AllReadOnly<Priority>().AnyAsync(p => p.Id == priorityId);
        }

        public async Task<TaskDetailsServiceModel?> GetTaskDetailsByIdAsync(int taskId)
        {
	        return await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
		        .Where(t => t.Id == taskId)
		        .Select(t => new TaskDetailsServiceModel()
		        {
			        Id = t.Id,
			        Name = t.Name,
			        ProjectNumber = t.Project.ProjectNumber,
			        Description = t.Description,
			        Status = t.TaskStatus.Name,
			        Priority = t.Priority.Name,
			        StartDate = t.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
			        EndDate = t.EndDate != null ? t.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
			        Deadline = t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
			        AssignedMachineName = t.Machine != null ? t.Machine.Name : string.Empty,
			        Creator = t.Creator.UserName
		        })
		        .FirstOrDefaultAsync();
        }

        public async Task<TaskFormModel?> GetTaskForEditByIdAsync(int taskId)
        {
	        var model = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
		        .Where(t => t.Id == taskId)
		        .Select(t => new TaskFormModel()
		        {
                    Name = t.Name,
                    Description = t.Description,
                    PriorityId = t.PriorityId,
                    StatusId = t.TaskStatusId,
                    ProjectNumber = t.Project.ProjectNumber,
                    StartDate = t.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                    EndDate = t.EndDate != null ? t.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    Deadline = t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
				})
		        .FirstOrDefaultAsync();

	        if (model != null)
	        {
		        model.Priorities = await GetAllPrioritiesAsync();
		        model.Statuses = await GetAllStatusesAsync();
	        }

	        return model;
        }

        public async Task EditTaskAsync(TaskFormModel model, 
	        int taskId, 
	        DateTime parsedStartDate,
	        DateTime? parsedEndDate,
	        DateTime? parsedDeadline,
	        int projectId)
        {
			var task = await _repository.GetByIdAsync<Infrastructure.Data.Models.Task>(taskId);
			
			if (task != null)
	        {
		        task.Name = model.Name;
		        task.Description = model.Description;
		        task.StartDate = parsedStartDate;
		        task.EndDate = parsedEndDate;
		        task.DeadLine = parsedDeadline;
		        task.ProjectId = projectId;
		        task.PriorityId = model.PriorityId;
		        task.TaskStatusId = model.StatusId;

		        await _repository.SaveChangesAsync();
	        }
        }

        public async Task<TaskDeleteServiceModel?> GetTaskForDeleteByIdAsync(int taskId)
        {
	        return await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
		        .Where(t => t.Id == taskId)
		        .Select(t => new TaskDeleteServiceModel()
		        {
			        Id = t.Id,
			        Description = t.Description,
			        Name = t.Name,
			        Creator = t.Creator.UserName
		        })
		        .FirstOrDefaultAsync();
        }

        public async Task DeleteTaskAsync(int taskId)
        {
	        var taskToDelete = await _repository.GetByIdAsync<Infrastructure.Data.Models.Task>(taskId);

	        if (taskToDelete != null)
	        {
		        await _repository.DeleteAsync<Infrastructure.Data.Models.Task>(taskToDelete.Id);
		        await _repository.SaveChangesAsync();
	        }
        }

        public async Task<bool> TaskExistByIdAsync(int taskId)
        {
	        return await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
		        .AnyAsync(t => t.Id == taskId);
        }
    }
}
