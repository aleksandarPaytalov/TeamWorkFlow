using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Exceptions;
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

        public async Task<ICollection<TaskServiceModel>> GetMyTasksAsync(string userId)
        {
	        var operatorId = await GetOperatorIdByUserId(userId);

            var model = await _repository.AllReadOnly<TaskOperator>()
	            .Where(to => to.OperatorId == operatorId)
	            .Select(to => new TaskServiceModel()
	            {
					Id = to.Task.Id,
                    Name = to.Task.Name,
					Description = to.Task.Description,
					Status = to.Task.TaskStatus.Name,
					Priority = to.Task.Priority.Name,
					ProjectNumber = to.Task.Project.ProjectNumber,
					StartDate = to.Task.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
					EndDate = to.Task.EndDate != null ? to.Task.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
					Deadline = to.Task.DeadLine != null ? to.Task.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty
				})
	            .ToListAsync();

			return model;
        }

        public async Task AddTaskToMyCollection(TaskServiceModel model, string userId)
        {
	        var operatorId = await GetOperatorIdByUserId(userId);
	        var exist = await TaskExistInTaskOperatorTableByIdAsync(model.Id);

	        var alreadyInCollection = await _repository.AllReadOnly<TaskOperator>()
		        .FirstOrDefaultAsync(to => to.TaskId == model.Id && to.OperatorId == operatorId);

	        if (alreadyInCollection == null && exist == false)
	        {
		        var taskOperator = new TaskOperator()
		        {
			        TaskId = model.Id,
                    OperatorId = operatorId
		        };

		        await _repository.AddAsync(taskOperator);
		        await _repository.SaveChangesAsync();
	        }
        }

        public async Task<int> GetOperatorIdByUserId(string userId)
        {
	        var operatorModel = await _repository.AllReadOnly<Operator>()
		        .Where(o => o.UserId == userId)
		        .FirstOrDefaultAsync();

	        if (operatorModel == null)
	        {
		        throw new UnExistingActionException("The operator with this userId does not exist");
	        }

	        return operatorModel.Id;
        }

        public async Task<TaskServiceModel?> GetTaskByIdAsync(int id)
        {
	        var taskModel = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
		        .Where(t => t.Id == id)
		        .Select(t => new TaskServiceModel()
		        {
			        Id = t.Id,
			        Name = t.Name,
			        Description = t.Description,
			        Status = t.TaskStatus.Name,
			        Priority = t.Priority.Name,
			        ProjectNumber = t.Project.ProjectNumber,
			        StartDate = t.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
			        EndDate = t.EndDate != null
				        ? t.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture)
				        : string.Empty,
			        Deadline = t.DeadLine != null
				        ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture)
				        : string.Empty
		        })
		        .FirstOrDefaultAsync();

	        return taskModel;
        }

        public async Task<bool> TaskExistInTaskOperatorTableByIdAsync(int taskId)
        {
	        var exist = await _repository.AllReadOnly<TaskOperator>()
		        .Where(to => to.TaskId == taskId)
		        .AnyAsync();

	        return exist;
        }

        public async Task RemoveFromCollection(int taskId, string userId)
        {
	        var operatorId = await GetOperatorIdByUserId(userId);

	        var toRemoveFromCollection = await _repository.AllReadOnly<TaskOperator>()
		        .Where(to =>
			        to.TaskId == taskId &&
			        to.OperatorId == operatorId)
		        .FirstOrDefaultAsync();

	        if (toRemoveFromCollection != null)
	        {
		        _repository.DeleteTaskOperator(toRemoveFromCollection);
		        await _repository.SaveChangesAsync();
	        }
        }

        public async Task<ICollection<TaskServiceModel>> GetAllAssignedTasksAsync()
        {
	        var model = await _repository.AllReadOnly<TaskOperator>()
		        .Select(to => new TaskServiceModel()
				{
					Id = to.Task.Id,
					Name = to.Task.Name,
					Description = to.Task.Description,
					Status = to.Task.TaskStatus.Name,
					Priority = to.Task.Priority.Name,
					ProjectNumber = to.Task.Project.ProjectNumber,
					StartDate = to.Task.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
					EndDate = to.Task.EndDate != null ? to.Task.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
					Deadline = to.Task.DeadLine != null ? to.Task.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty
				})
				.ToListAsync();

			return model;
        }

        public async Task<int> GetOperatorIdByAssignedTaskId(int taskId)
        {
	        var operatorModel = await _repository.AllReadOnly<TaskOperator>()
		        .Where(to => to.TaskId == taskId)
		        .FirstOrDefaultAsync();

	        return operatorModel?.OperatorId ?? 0;
        }

        public async Task RemoveAssignedTaskFromUserCollection(int taskId, int operatorId)
        {
			var toRemoveAssignedTask = await _repository.AllReadOnly<TaskOperator>()
				.Where(to =>
					to.TaskId == taskId &&
					to.OperatorId == operatorId)
				.FirstOrDefaultAsync();

			if (toRemoveAssignedTask != null)
			{
				_repository.DeleteTaskOperator(toRemoveAssignedTask);
				await _repository.SaveChangesAsync();
			}
		}
    }
}
