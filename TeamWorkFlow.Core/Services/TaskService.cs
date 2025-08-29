using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Enumerations;
using TeamWorkFlow.Core.Exceptions;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Core.Models.Machine;
using TeamWorkFlow.Core.Models.Operator;
using TeamWorkFlow.Core.Models.BulkOperations;
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
                .Include(t => t.TaskStatus)
                .Include(t => t.CompletedBy)
                .Where(t => t.TaskStatus.Name.ToLower() != "finished") // Exclude finished tasks
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
                    Deadline = t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    ActualTime = t.ActualTime,
                    CompletedBy = t.CompletedBy != null ? t.CompletedBy.UserName : null,
                    EstimatedTime = t.EstimatedTime
				})
                .ToListAsync();
        }

        public async Task<TaskQueryServiceModel> AllAsync(
            TaskSorting sorting = TaskSorting.LastAdded,
            string? search = null,
            int tasksPerPage = 10,
            int currentPage = 1)
        {
            IQueryable<Infrastructure.Data.Models.Task> tasksToBeDisplayed = _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                .Include(t => t.TaskStatus)
                .Where(t => t.TaskStatus.Name.ToLower() != "finished"); // Exclude finished tasks from main list

            if (!string.IsNullOrWhiteSpace(search))
            {
                string normalizedSearch = search.ToLower();
                tasksToBeDisplayed = tasksToBeDisplayed
                    .Where(t => t.Name.ToLower().Contains(normalizedSearch) ||
                               t.Description.ToLower().Contains(normalizedSearch) ||
                               t.Project.ProjectNumber.ToLower().Contains(normalizedSearch));
            }

            tasksToBeDisplayed = sorting switch
            {
                TaskSorting.NameAscending => tasksToBeDisplayed.OrderBy(t => t.Name),
                TaskSorting.NameDescending => tasksToBeDisplayed.OrderByDescending(t => t.Name),
                TaskSorting.ProjectNumberAscending => tasksToBeDisplayed.OrderBy(t => t.Project.ProjectNumber),
                TaskSorting.ProjectNumberDescending => tasksToBeDisplayed.OrderByDescending(t => t.Project.ProjectNumber),
                TaskSorting.StartDateAscending => tasksToBeDisplayed.OrderBy(t => t.StartDate),
                TaskSorting.StartDateDescending => tasksToBeDisplayed.OrderByDescending(t => t.StartDate),
                TaskSorting.DeadlineAscending => tasksToBeDisplayed.OrderBy(t => t.DeadLine),
                TaskSorting.DeadlineDescending => tasksToBeDisplayed.OrderByDescending(t => t.DeadLine),
                _ => tasksToBeDisplayed.OrderByDescending(t => t.Id)
            };

            var tasks = await tasksToBeDisplayed
                .Include(t => t.Machine)
                .Include(t => t.TasksOperators)
                .ThenInclude(to => to.Operator)
                .Include(t => t.CompletedBy)
                .Skip((currentPage - 1) * tasksPerPage)
                .Take(tasksPerPage)
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
                    Deadline = t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    EstimatedTime = t.EstimatedTime,
                    ActualTime = t.ActualTime,
                    CompletedBy = t.CompletedBy != null ? t.CompletedBy.UserName : null,
                    MachineId = t.MachineId,
                    MachineName = t.Machine != null ? t.Machine.Name : null,
                    Operators = t.TasksOperators.Select(to => new TaskOperatorModel
                    {
                        OperatorId = to.OperatorId,
                        OperatorName = to.Operator.FullName
                    }).ToList()
                })
                .ToListAsync();

            int totalTasks = await tasksToBeDisplayed.CountAsync();

            return new TaskQueryServiceModel()
            {
                Tasks = tasks,
                TotalTasksCount = totalTasks
            };
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
                ProjectId = projectId,
                EstimatedTime = model.EstimatedTime
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
                    EstimatedTime = t.EstimatedTime
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
		        task.EstimatedTime = model.EstimatedTime;

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
	            .Include(to => to.Task.CompletedBy)
	            .Where(to => to.OperatorId == operatorId && to.Task.TaskStatus.Name.ToLower() != "finished") // Exclude finished tasks
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
					Deadline = to.Task.DeadLine != null ? to.Task.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
					ActualTime = to.Task.ActualTime,
					CompletedBy = to.Task.CompletedBy != null ? to.Task.CompletedBy.UserName : null,
					EstimatedTime = to.Task.EstimatedTime
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
		        .Include(t => t.CompletedBy)
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
				        : string.Empty,
			        EstimatedTime = t.EstimatedTime,
			        ActualTime = t.ActualTime,
			        CompletedBy = t.CompletedBy != null ? t.CompletedBy.UserName : null
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
		        .Include(to => to.Task.CompletedBy)
		        .Where(to => to.Task.TaskStatus.Name.ToLower() != "finished") // Exclude finished tasks
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
					ActualTime = to.Task.ActualTime,
					CompletedBy = to.Task.CompletedBy != null ? to.Task.CompletedBy.UserName : null,
					Deadline = to.Task.DeadLine != null ? to.Task.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
					EstimatedTime = to.Task.EstimatedTime
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

        public async Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetAllAssignedTasksAsync(int page, int pageSize)
        {
            var query = _repository.AllReadOnly<TaskOperator>()
                .Include(to => to.Task.CompletedBy)
                .Where(to => to.Task.TaskStatus.Name.ToLower() != "finished"); // Exclude finished tasks
            var totalCount = await query.CountAsync();
            var tasks = await query
                .OrderBy(to => to.Task.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(to => new TaskServiceModel
                {
                    Id = to.Task.Id,
                    Name = to.Task.Name,
                    Description = to.Task.Description,
                    Status = to.Task.TaskStatus.Name,
                    Priority = to.Task.Priority.Name,
                    ProjectNumber = to.Task.Project.ProjectNumber,
                    StartDate = to.Task.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                    EndDate = to.Task.EndDate != null ? to.Task.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    Deadline = to.Task.DeadLine != null ? to.Task.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    ActualTime = to.Task.ActualTime,
                    CompletedBy = to.Task.CompletedBy != null ? to.Task.CompletedBy.UserName : null
                })
                .ToListAsync();
            return (tasks, totalCount);
        }

        public async Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetAllTasksAsync(int page, int pageSize)
        {
            var query = _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                .Include(t => t.TaskStatus)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Machine)
                .Include(t => t.TasksOperators)
                .ThenInclude(to => to.Operator)
                .Include(t => t.CompletedBy)
                .Where(t => t.TaskStatus.Name.ToLower() != "finished"); // Exclude finished tasks from main list

            var totalCount = await query.CountAsync();
            var tasks = await query
                .OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaskServiceModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Status = t.TaskStatus.Name,
                    Priority = t.Priority.Name,
                    ProjectNumber = t.Project.ProjectNumber,
                    StartDate = t.StartDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                    EndDate = t.EndDate != null ? t.EndDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    Deadline = t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    EstimatedTime = t.EstimatedTime,
                    ActualTime = t.ActualTime,
                    CompletedBy = t.CompletedBy != null ? t.CompletedBy.UserName : null,
                    MachineId = t.MachineId,
                    MachineName = t.Machine != null ? t.Machine.Name : null,
                    Operators = t.TasksOperators.Select(to => new TaskOperatorModel
                    {
                        OperatorId = to.OperatorId,
                        OperatorName = to.Operator.FullName
                    }).ToList()
                })
                .ToListAsync();
            return (tasks, totalCount);
        }

        // Archive methods
        public async Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetArchivedTasksAsync(int page, int pageSize)
        {
            return await GetArchivedTasksAsync(page, pageSize, null, TaskSorting.LastAdded);
        }

        public async Task<(ICollection<TaskServiceModel> Tasks, int TotalCount)> GetArchivedTasksAsync(int page, int pageSize, string? search, TaskSorting sorting)
        {
            var currentPage = page;
            var tasksPerPage = pageSize;

            var archivedTasksQuery = _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                .Include(t => t.TaskStatus)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Machine)
                .Include(t => t.TasksOperators)
                .ThenInclude(to => to.Operator)
                .Include(t => t.CompletedBy)
                .Where(t => t.TaskStatus.Name.ToLower() == "finished"); // Only finished tasks

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                string normalizedSearch = search.ToLower();
                archivedTasksQuery = archivedTasksQuery.Where(t =>
                    t.Name.ToLower().Contains(normalizedSearch) ||
                    t.Description.ToLower().Contains(normalizedSearch) ||
                    t.Project.ProjectNumber.ToLower().Contains(normalizedSearch));
            }

            // Apply sorting
            archivedTasksQuery = sorting switch
            {
                TaskSorting.NameAscending => archivedTasksQuery.OrderBy(t => t.Name),
                TaskSorting.NameDescending => archivedTasksQuery.OrderByDescending(t => t.Name),
                TaskSorting.ProjectNumberAscending => archivedTasksQuery.OrderBy(t => t.Project.ProjectNumber),
                TaskSorting.ProjectNumberDescending => archivedTasksQuery.OrderByDescending(t => t.Project.ProjectNumber),
                TaskSorting.StartDateAscending => archivedTasksQuery.OrderBy(t => t.StartDate),
                TaskSorting.StartDateDescending => archivedTasksQuery.OrderByDescending(t => t.StartDate),
                TaskSorting.DeadlineAscending => archivedTasksQuery.OrderBy(t => t.DeadLine),
                TaskSorting.DeadlineDescending => archivedTasksQuery.OrderByDescending(t => t.DeadLine),
                _ => archivedTasksQuery.OrderByDescending(t => t.EndDate ?? t.StartDate) // Default: most recently finished first
            };

            var totalCount = await archivedTasksQuery.CountAsync();

            var tasks = await archivedTasksQuery
                .Skip((currentPage - 1) * tasksPerPage)
                .Take(tasksPerPage)
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
                    Deadline = t.DeadLine != null ? t.DeadLine.Value.ToString(DateFormat, CultureInfo.InvariantCulture) : string.Empty,
                    EstimatedTime = t.EstimatedTime,
                    ActualTime = t.ActualTime,
                    CompletedBy = t.CompletedBy != null ? t.CompletedBy.UserName : null,
                    MachineId = t.MachineId,
                    MachineName = t.Machine != null ? t.Machine.Name : null,
                    Operators = t.TasksOperators.Select(to => new TaskOperatorModel
                    {
                        OperatorId = to.OperatorId,
                        OperatorName = to.Operator.FullName
                    }).ToList()
                })
                .ToListAsync();

            return (tasks, totalCount);
        }

        // Machine assignment methods
        public async Task<(bool Success, string Message)> AssignMachineToTaskAsync(int taskId, int machineId)
        {
            var validation = await ValidateMachineAssignmentAsync(taskId, machineId);
            if (!validation.CanAssign)
            {
                return (false, validation.Reason);
            }

            var task = await _repository.GetByIdAsync<Infrastructure.Data.Models.Task>(taskId);
            if (task == null)
            {
                return (false, "Task not found");
            }

            task.MachineId = machineId;
            await _repository.SaveChangesAsync();

            return (true, "Machine assigned successfully");
        }

        public async Task<(bool Success, string Message)> UnassignMachineFromTaskAsync(int taskId)
        {
            var task = await _repository.GetByIdAsync<Infrastructure.Data.Models.Task>(taskId);
            if (task == null)
            {
                return (false, "Task not found");
            }

            task.MachineId = null;
            await _repository.SaveChangesAsync();

            return (true, "Machine unassigned successfully");
        }

        public async Task<ICollection<MachineServiceModel>> GetAvailableMachinesForTaskAsync(int taskId)
        {
            return await _repository.AllReadOnly<Machine>()
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.TaskStatus)
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.TasksOperators)
                    .ThenInclude(to => to.Operator)
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.Project)
                .Where(m => m.IsCalibrated && !m.Tasks.Any(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished"))
                .Select(m => new MachineServiceModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    IsCalibrated = m.IsCalibrated,
                    CalibrationSchedule = m.CalibrationSchedule.ToString(DateFormat),
                    Capacity = m.Capacity,
                    IsOccupied = m.Tasks.Any(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished"),
                    AssignedTaskId = m.Tasks.Where(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished").Select(t => (int?)t.Id).FirstOrDefault(),
                    AssignedTaskName = m.Tasks.Where(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Name).FirstOrDefault(),
                    AssignedTaskProjectNumber = m.Tasks.Where(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished").Select(t => t.Project.ProjectNumber).FirstOrDefault(),
                    TaskStatus = m.Tasks.Where(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished").Select(t => t.TaskStatus.Name).FirstOrDefault(),
                    AssignedOperatorNames = string.Join(", ", m.Tasks
                        .Where(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished")
                        .SelectMany(t => t.TasksOperators)
                        .Select(to => to.Operator.FullName))
                })
                .ToListAsync();
        }

        public async Task<(bool CanAssign, string Reason)> ValidateMachineAssignmentAsync(int taskId, int machineId)
        {
            var machine = await _repository.AllReadOnly<Machine>()
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.TaskStatus)
                .Include(m => m.Tasks)
                    .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == machineId);

            if (machine == null)
            {
                return (false, "Machine not found");
            }

            if (!machine.IsCalibrated)
            {
                return (false, "Machine is not calibrated and cannot be assigned to tasks");
            }

            // Check if machine is already assigned to another active (non-finished) task
            var assignedTask = machine.Tasks
                .Where(t => t.Id != taskId && t.TaskStatus.Name.ToLower() != "finished")
                .FirstOrDefault();
                
            if (assignedTask != null)
            {
                return (false, $"Machine is already assigned to task '{assignedTask.Name}' (Project #{assignedTask.Project?.ProjectNumber})");
            }

            return (true, "Machine can be assigned");
        }

        // Operator assignment methods
        public async Task<(bool Success, string Message)> AssignOperatorToTaskAsync(int taskId, int operatorId)
        {
            var taskExists = await _repository.AllReadOnly<Infrastructure.Data.Models.Task>()
                .AnyAsync(t => t.Id == taskId);

            if (!taskExists)
            {
                return (false, "Task not found");
            }

            var operatorExists = await _repository.AllReadOnly<Operator>()
                .AnyAsync(o => o.Id == operatorId && o.IsActive);

            if (!operatorExists)
            {
                return (false, "Operator not found or inactive");
            }

            // Check if operator is already assigned to this task
            var existingAssignment = await _repository.AllReadOnly<TaskOperator>()
                .AnyAsync(to => to.TaskId == taskId && to.OperatorId == operatorId);

            if (existingAssignment)
            {
                return (false, "Operator is already assigned to this task");
            }

            var taskOperator = new TaskOperator
            {
                TaskId = taskId,
                OperatorId = operatorId
            };

            await _repository.AddAsync(taskOperator);
            await _repository.SaveChangesAsync();

            return (true, "Operator assigned successfully");
        }

        public async Task<(bool Success, string Message)> UnassignOperatorFromTaskAsync(int taskId, int operatorId)
        {
            var taskOperator = await _repository.AllReadOnly<TaskOperator>()
                .FirstOrDefaultAsync(to => to.TaskId == taskId && to.OperatorId == operatorId);

            if (taskOperator == null)
            {
                return (false, "Operator assignment not found");
            }

            _repository.DeleteTaskOperator(taskOperator);
            await _repository.SaveChangesAsync();

            return (true, "Operator unassigned successfully");
        }

        public async Task<ICollection<OperatorServiceModel>> GetAvailableOperatorsForTaskAsync(int taskId)
        {
            return await _repository.AllReadOnly<Operator>()
                .Include(o => o.AvailabilityStatus)
                .Where(o => o.IsActive)
                .Select(o => new OperatorServiceModel
                {
                    Id = o.Id,
                    FullName = o.FullName,
                    Email = o.Email,
                    PhoneNumber = o.PhoneNumber,
                    IsActive = o.IsActive,
                    Capacity = o.Capacity,
                    AvailabilityStatus = o.AvailabilityStatus.Name
                })
                .ToListAsync();
        }

        public async Task<ICollection<OperatorServiceModel>> GetAssignedOperatorsForTaskAsync(int taskId)
        {
            return await _repository.AllReadOnly<TaskOperator>()
                .Include(to => to.Operator)
                .ThenInclude(o => o.AvailabilityStatus)
                .Where(to => to.TaskId == taskId)
                .Select(to => new OperatorServiceModel
                {
                    Id = to.Operator.Id,
                    FullName = to.Operator.FullName,
                    Email = to.Operator.Email,
                    PhoneNumber = to.Operator.PhoneNumber,
                    IsActive = to.Operator.IsActive,
                    Capacity = to.Operator.Capacity,
                    AvailabilityStatus = to.Operator.AvailabilityStatus.Name
                })
                .ToListAsync();
        }

        // Estimated time management
        public async Task<(bool Success, string Message)> SetEstimatedTimeAsync(int taskId, int estimatedTime)
        {
            if (estimatedTime < 1 || estimatedTime > 1000)
            {
                return (false, "Estimated time must be between 1 and 1000 hours");
            }

            var task = await _repository.GetByIdAsync<Infrastructure.Data.Models.Task>(taskId);
            if (task == null)
            {
                return (false, "Task not found");
            }

            task.EstimatedTime = estimatedTime;
            await _repository.SaveChangesAsync();

            return (true, "Estimated time updated successfully");
        }

        // Task status management
        public async Task<(bool Success, string Message)> ChangeTaskStatusAsync(int taskId, int statusId, string userId)
        {
            var task = await _repository.GetByIdAsync<Infrastructure.Data.Models.Task>(taskId);
            if (task == null)
            {
                return (false, "Task not found");
            }

            var statusExists = await TaskStatusExistsAsync(statusId);
            if (!statusExists)
            {
                return (false, "Invalid status");
            }

            var previousStatusId = task.TaskStatusId;
            task.TaskStatusId = statusId;

            // Handle status transitions
            var now = DateTime.Now;

            // When changing from Open (1) to In Progress (2), set start date
            if (previousStatusId == 1 && statusId == 2)
            {
                task.StartDate = now;
            }
            // When changing to Finished (3), set end date, calculate actual time, and track completion user
            else if (statusId == 3)
            {
                task.EndDate = now;
                task.CompletedById = userId;

                // Calculate actual time in hours if start date is valid
                if (task.StartDate != default(DateTime))
                {
                    var timeSpan = now - task.StartDate;
                    // Only calculate if the time span is reasonable (not negative and not too large)
                    if (timeSpan.TotalHours >= 0 && timeSpan.TotalDays <= 365)
                    {
                        task.ActualTime = Math.Round(timeSpan.TotalHours, 2);
                    }
                }
            }

            await _repository.SaveChangesAsync();

            var statusName = await _repository.AllReadOnly<TaskStatus>()
                .Where(s => s.Id == statusId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            return (true, $"Task status changed to {statusName}");
        }

        // Bulk operations
        public async Task<BulkOperationResult> BulkDeleteAsync(List<int> taskIds)
        {
            var result = new BulkOperationResult
            {
                TotalItems = taskIds.Count
            };

            if (!taskIds.Any())
            {
                result.Success = false;
                result.ErrorMessages.Add("No tasks selected for deletion");
                return result;
            }

            var validationErrors = new List<string>();
            var processedCount = 0;

            foreach (var taskId in taskIds)
            {
                try
                {
                    var taskExists = await TaskExistByIdAsync(taskId);
                    if (!taskExists)
                    {
                        validationErrors.Add($"Task with ID {taskId} not found");
                        continue;
                    }

                    await DeleteTaskAsync(taskId);
                    processedCount++;
                }
                catch (Exception ex)
                {
                    validationErrors.Add($"Failed to delete task {taskId}: {ex.Message}");
                }
            }

            result.ProcessedItems = processedCount;
            result.FailedItems = taskIds.Count - processedCount;
            result.ErrorMessages = validationErrors;
            result.Success = processedCount > 0;
            result.Message = $"Successfully deleted {processedCount} out of {taskIds.Count} tasks";

            return result;
        }

        public async Task<BulkOperationResult> BulkArchiveAsync(List<int> taskIds)
        {
            var result = new BulkOperationResult
            {
                TotalItems = taskIds.Count
            };

            if (!taskIds.Any())
            {
                result.Success = false;
                result.ErrorMessages.Add("No tasks selected for archiving");
                return result;
            }

            // Get the "finished" status ID
            var finishedStatus = await _repository.AllReadOnly<TaskStatus>()
                .Where(s => s.Name.ToLower() == "finished")
                .FirstOrDefaultAsync();

            if (finishedStatus == null)
            {
                result.Success = false;
                result.ErrorMessages.Add("Finished status not found in the system");
                return result;
            }

            var validationErrors = new List<string>();
            var processedCount = 0;

            foreach (var taskId in taskIds)
            {
                try
                {
                    var task = await _repository.GetByIdAsync<Infrastructure.Data.Models.Task>(taskId);
                    if (task == null)
                    {
                        validationErrors.Add($"Task with ID {taskId} not found");
                        continue;
                    }

                    // Check if task is already finished
                    if (task.TaskStatusId == finishedStatus.Id)
                    {
                        validationErrors.Add($"Task {taskId} is already archived (finished)");
                        continue;
                    }

                    // Archive the task by setting status to finished
                    task.TaskStatusId = finishedStatus.Id;
                    processedCount++;
                }
                catch (Exception ex)
                {
                    validationErrors.Add($"Failed to archive task {taskId}: {ex.Message}");
                }
            }

            if (processedCount > 0)
            {
                await _repository.SaveChangesAsync();
            }

            result.ProcessedItems = processedCount;
            result.FailedItems = taskIds.Count - processedCount;
            result.ErrorMessages = validationErrors;
            result.Success = processedCount > 0;
            result.Message = $"Successfully archived {processedCount} out of {taskIds.Count} tasks";

            return result;
        }
    }
}
