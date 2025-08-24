using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Sprint;
using TeamWorkFlow.Infrastructure.Data;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Core.Services
{
    public class SprintService : ISprintService
    {
        private readonly TeamWorkFlowDbContext context;

        public SprintService(TeamWorkFlowDbContext context)
        {
            this.context = context;
        }

        public async Task<SprintPlanningViewModel> GetSprintPlanningDataAsync(
            string searchTerm = "",
            string statusFilter = "",
            string priorityFilter = "",
            string projectFilter = "",
            string operatorFilter = "",
            string machineFilter = "",
            int page = 1,
            int pageSize = 20)
        {
            var viewModel = new SprintPlanningViewModel
            {
                SearchTerm = searchTerm,
                StatusFilter = statusFilter,
                PriorityFilter = priorityFilter,
                ProjectFilter = projectFilter,
                OperatorFilter = operatorFilter,
                MachineFilter = machineFilter,
                CurrentPage = page,
                PageSize = pageSize
            };

            // Get sprint tasks
            viewModel.SprintTasks = await GetSprintTasksAsync();

            // Get backlog tasks with filtering
            var backlogQuery = GetBacklogTasksQuery();
            
            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
            {
                backlogQuery = backlogQuery.Where(t => 
                    t.Name.Contains(searchTerm) || 
                    t.Description.Contains(searchTerm) ||
                    t.Project.ProjectName.Contains(searchTerm) ||
                    t.Project.ProjectNumber.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                backlogQuery = backlogQuery.Where(t => t.TaskStatus.Name == statusFilter);
            }

            if (!string.IsNullOrEmpty(priorityFilter))
            {
                backlogQuery = backlogQuery.Where(t => t.Priority.Name == priorityFilter);
            }

            if (!string.IsNullOrEmpty(projectFilter))
            {
                backlogQuery = backlogQuery.Where(t => t.Project.ProjectName == projectFilter);
            }

            if (!string.IsNullOrEmpty(machineFilter))
            {
                backlogQuery = backlogQuery.Where(t => t.Machine != null && t.Machine.Name == machineFilter);
            }

            // Get total count for pagination
            viewModel.TotalTasks = await backlogQuery.CountAsync();

            // Apply pagination and get backlog tasks
            var backlogTasks = await backlogQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            viewModel.BacklogTasks = await MapToSprintTaskModels(backlogTasks);

            // Get capacity, resources, and summary
            viewModel.Capacity = await GetSprintCapacityAsync();
            viewModel.Resources = await GetResourceAvailabilityAsync();
            viewModel.Summary = await GetSprintSummaryAsync();
            viewModel.Timeline = await GetSprintTimelineAsync(
                viewModel.SprintStartDate, 
                viewModel.SprintEndDate);

            return viewModel;
        }

        public async Task<List<SprintTaskServiceModel>> GetSprintTasksAsync()
        {
            var sprintTasks = await context.Tasks
                .Include(t => t.TaskStatus)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Machine)
                .Include(t => t.Creator)
                .Include(t => t.TasksOperators)
                    .ThenInclude(to => to.Operator)
                .Where(t => t.IsInSprint)
                .OrderBy(t => t.SprintOrder)
                .ToListAsync();

            return await MapToSprintTaskModels(sprintTasks);
        }

        public async Task<List<SprintTaskServiceModel>> GetBacklogTasksAsync()
        {
            var backlogTasks = await GetBacklogTasksQuery().ToListAsync();
            return await MapToSprintTaskModels(backlogTasks);
        }

        private IQueryable<Infrastructure.Data.Models.Task> GetBacklogTasksQuery()
        {
            return context.Tasks
                .Include(t => t.TaskStatus)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Machine)
                .Include(t => t.Creator)
                .Include(t => t.TasksOperators)
                    .ThenInclude(to => to.Operator)
                .Where(t => !t.IsInSprint && t.TaskStatus.Name != "Finished")
                .OrderByDescending(t => t.Priority.Name == "Critical")
                .ThenByDescending(t => t.Priority.Name == "High")
                .ThenBy(t => t.DeadLine)
                .ThenBy(t => t.StartDate);
        }

        public async Task<bool> AddTaskToSprintAsync(int taskId, int sprintOrder)
        {
            try
            {
                var task = await context.Tasks.FindAsync(taskId);
                if (task == null) return false;

                // Validate capacity before adding
                var (canAdd, reason) = await ValidateTaskForSprintAsync(taskId);
                if (!canAdd) return false;

                task.IsInSprint = true;
                task.SprintOrder = sprintOrder;
                task.PlannedStartDate = DateTime.Today;
                task.PlannedEndDate = DateTime.Today.AddDays(Math.Max(1, task.EstimatedTime / 8));

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveTaskFromSprintAsync(int taskId)
        {
            try
            {
                var task = await context.Tasks.FindAsync(taskId);
                if (task == null) return false;

                task.IsInSprint = false;
                task.SprintOrder = 0;
                task.PlannedStartDate = null;
                task.PlannedEndDate = null;

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateSprintTaskOrderAsync(Dictionary<int, int> taskOrders)
        {
            try
            {
                foreach (var kvp in taskOrders)
                {
                    var task = await context.Tasks.FindAsync(kvp.Key);
                    if (task != null)
                    {
                        task.SprintOrder = kvp.Value;
                    }
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskEstimatedTimeAsync(int taskId, int estimatedTime)
        {
            try
            {
                var task = await context.Tasks.FindAsync(taskId);
                if (task == null) return false;

                task.EstimatedTime = estimatedTime;
                
                // Update planned end date if task is in sprint
                if (task.IsInSprint && task.PlannedStartDate.HasValue)
                {
                    task.PlannedEndDate = task.PlannedStartDate.Value.AddDays(Math.Max(1, estimatedTime / 8));
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskPlannedDatesAsync(int taskId, DateTime? plannedStartDate, DateTime? plannedEndDate)
        {
            try
            {
                var task = await context.Tasks.FindAsync(taskId);
                if (task == null) return false;

                task.PlannedStartDate = plannedStartDate;
                task.PlannedEndDate = plannedEndDate;

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<SprintCapacityServiceModel> GetSprintCapacityAsync()
        {
            var capacity = new SprintCapacityServiceModel();

            // Get active operators
            var operators = await context.Operators
                .Include(o => o.AvailabilityStatus)
                .Where(o => o.IsActive)
                .ToListAsync();

            // Get Calibrated machines
            var machines = await context.Machines
                .Where(m => m.IsCalibrated)
                .ToListAsync();

            // Get sprint tasks
            var sprintTasks = await context.Tasks
                .Include(t => t.TasksOperators)
                .Where(t => t.IsInSprint)
                .ToListAsync();

            // Calculate operator capacities
            foreach (var op in operators)
            {
                var operatorCapacity = new OperatorCapacityModel
                {
                    Id = op.Id,
                    FullName = op.FullName,
                    IsActive = op.IsActive,
                    AvailabilityStatus = op.AvailabilityStatus?.Name ?? "Unknown",
                    AvailableHours = op.AvailabilityStatus?.Name == "at work" ? 40 : 0, // 40 hours per week
                    AssignedHours = sprintTasks
                        .Where(t => t.TasksOperators.Any(to => to.OperatorId == op.Id))
                        .Sum(t => t.EstimatedTime)
                };

                capacity.OperatorCapacities.Add(operatorCapacity);
            }

            // Calculate machine capacities
            foreach (var machine in machines)
            {
                var machineCapacity = new MachineCapacityModel
                {
                    Id = machine.Id,
                    Name = machine.Name,
                    IsActive = machine.IsCalibrated,
                    AvailableHours = machine.Capacity * 5, // Weekly capacity = daily capacity * 5 working days
                    AssignedHours = sprintTasks
                        .Where(t => t.MachineId == machine.Id)
                        .Sum(t => t.EstimatedTime)
                };

                capacity.MachineCapacities.Add(machineCapacity);
            }

            // Calculate totals
            capacity.TotalOperatorHours = capacity.OperatorCapacities.Sum(o => o.AvailableHours);
            capacity.TotalMachineHours = capacity.MachineCapacities.Sum(m => m.AvailableHours);
            capacity.RequiredOperatorHours = sprintTasks.Sum(t => t.EstimatedTime);
            capacity.RequiredMachineHours = sprintTasks.Where(t => t.MachineId.HasValue).Sum(t => t.EstimatedTime);
            capacity.AvailableOperators = operators.Count(o => o.AvailabilityStatus?.Name == "at work");
            capacity.AvailableMachines = machines.Count;

            return capacity;
        }

        public async Task<SprintResourceServiceModel> GetResourceAvailabilityAsync()
        {
            var resources = new SprintResourceServiceModel();

            // Get operators with their current assignments
            var operators = await context.Operators
                .Include(o => o.AvailabilityStatus)
                .Include(o => o.TasksOperators)
                    .ThenInclude(to => to.Task)
                .ToListAsync();

            foreach (var op in operators)
            {
                var sprintOperator = new SprintOperatorModel
                {
                    Id = op.Id,
                    FullName = op.FullName,
                    Email = op.Email,
                    IsActive = op.IsActive,
                    AvailabilityStatus = op.AvailabilityStatus?.Name ?? "Unknown",
                    WeeklyCapacity = op.AvailabilityStatus?.Name == "at work" ? 40 : 0,
                    CurrentAssignedHours = op.TasksOperators
                        .Where(to => to.Task.IsInSprint)
                        .Sum(to => to.Task.EstimatedTime)
                };

                resources.Operators.Add(sprintOperator);
            }

            // Get machines with their current assignments
            var machines = await context.Machines
                .Include(m => m.Tasks.Where(t => t.IsInSprint))
                .ToListAsync();

            foreach (var machine in machines)
            {
                var sprintMachine = new SprintMachineModel
                {
                    Id = machine.Id,
                    Name = machine.Name,
                    IsActive = machine.IsCalibrated,
                    WeeklyCapacity = machine.Capacity * 5, // Weekly capacity = daily capacity * 5 working days
                    CurrentAssignedHours = machine.Tasks.Sum(t => t.EstimatedTime)
                };

                resources.Machines.Add(sprintMachine);
            }

            return resources;
        }

        public async Task<List<SprintTimelineModel>> GetSprintTimelineAsync(DateTime startDate, DateTime endDate)
        {
            var timeline = new List<SprintTimelineModel>();
            var sprintTasks = await context.Tasks
                .Include(t => t.TaskStatus)
                .Include(t => t.Priority)
                .Where(t => t.IsInSprint)
                .ToListAsync();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dayModel = new SprintTimelineModel
                {
                    Date = date,
                    TasksStarting = sprintTasks
                        .Where(t => t.PlannedStartDate?.Date == date.Date)
                        .Select(t => MapToSprintTaskModel(t))
                        .ToList(),
                    TasksEnding = sprintTasks
                        .Where(t => t.PlannedEndDate?.Date == date.Date)
                        .Select(t => MapToSprintTaskModel(t))
                        .ToList(),
                    TasksInProgress = sprintTasks
                        .Where(t => t.PlannedStartDate?.Date <= date.Date &&
                                   t.PlannedEndDate?.Date >= date.Date)
                        .Select(t => MapToSprintTaskModel(t))
                        .ToList()
                };

                dayModel.TotalHoursScheduled = dayModel.TasksInProgress.Sum(t => t.EstimatedTime / 8);
                dayModel.IsOverloaded = dayModel.TotalHoursScheduled > 8;

                timeline.Add(dayModel);
            }

            return timeline;
        }

        public async Task<(bool CanAdd, string Reason)> ValidateTaskForSprintAsync(int taskId)
        {
            var task = await context.Tasks
                .Include(t => t.TasksOperators)
                .Include(t => t.Machine)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                return (false, "Task not found");

            if (task.IsInSprint)
                return (false, "Task is already in sprint");

            var capacity = await GetSprintCapacityAsync();

            // Check operator capacity
            var requiredOperatorHours = task.EstimatedTime;
            if (capacity.RequiredOperatorHours + requiredOperatorHours > capacity.TotalOperatorHours)
                return (false, "Insufficient operator capacity");

            // Check machine capacity if task requires a machine
            if (task.MachineId.HasValue)
            {
                var requiredMachineHours = task.EstimatedTime;
                if (capacity.RequiredMachineHours + requiredMachineHours > capacity.TotalMachineHours)
                    return (false, "Insufficient machine capacity");
            }

            return (true, "Task can be added to sprint");
        }

        public async Task<SprintSummaryModel> GetSprintSummaryAsync()
        {
            var sprintTasks = await context.Tasks
                .Include(t => t.TaskStatus)
                .Include(t => t.Priority)
                .Where(t => t.IsInSprint)
                .ToListAsync();

            var summary = new SprintSummaryModel
            {
                TotalTasksInSprint = sprintTasks.Count,
                CompletedTasks = sprintTasks.Count(t => t.TaskStatus.Name == "Finished"),
                InProgressTasks = sprintTasks.Count(t => t.TaskStatus.Name == "In Progress"),
                NotStartedTasks = sprintTasks.Count(t => t.TaskStatus.Name == "Not Started"),
                OnHoldTasks = sprintTasks.Count(t => t.TaskStatus.Name == "On Hold"),
                TotalEstimatedHours = sprintTasks.Sum(t => t.EstimatedTime),
                TotalActualHours = 0, // ActualHours not available in current model
                HighPriorityTasks = sprintTasks.Count(t => t.Priority.Name == "High"),
                CriticalPriorityTasks = sprintTasks.Count(t => t.Priority.Name == "Critical"),
                OverdueTasks = sprintTasks.Count(t =>
                    t.PlannedEndDate.HasValue &&
                    t.PlannedEndDate < DateTime.Today &&
                    t.TaskStatus.Name != "Finished")
            };

            return summary;
        }

        public async Task<bool> ClearSprintAsync()
        {
            try
            {
                var sprintTasks = await context.Tasks
                    .Where(t => t.IsInSprint)
                    .ToListAsync();

                foreach (var task in sprintTasks)
                {
                    task.IsInSprint = false;
                    task.SprintOrder = 0;
                    task.PlannedStartDate = null;
                    task.PlannedEndDate = null;
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> AutoAssignTasksToSprintAsync(int maxTasks = 10)
        {
            var capacity = await GetSprintCapacityAsync();
            var backlogTasks = await GetBacklogTasksQuery()
                .Take(maxTasks * 2) // Get more tasks to choose from
                .ToListAsync();

            int assignedCount = 0;
            int currentOrder = (await context.Tasks.Where(t => t.IsInSprint).MaxAsync(t => (int?)t.SprintOrder) ?? 0) + 1;

            foreach (var task in backlogTasks)
            {
                if (assignedCount >= maxTasks) break;

                var (canAdd, reason) = await ValidateTaskForSprintAsync(task.Id);
                if (canAdd)
                {
                    await AddTaskToSprintAsync(task.Id, currentOrder++);
                    assignedCount++;
                }
            }

            return assignedCount;
        }

        private Task<List<SprintTaskServiceModel>> MapToSprintTaskModels(List<Infrastructure.Data.Models.Task> tasks)
        {
            var result = new List<SprintTaskServiceModel>();

            foreach (var task in tasks)
            {
                result.Add(MapToSprintTaskModel(task));
            }

            return System.Threading.Tasks.Task.FromResult(result);
        }

        private SprintTaskServiceModel MapToSprintTaskModel(Infrastructure.Data.Models.Task task)
        {
            return new SprintTaskServiceModel
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                DeadLine = task.DeadLine,
                EstimatedTime = task.EstimatedTime,
                TaskStatus = task.TaskStatus?.Name ?? "Unknown",
                Priority = task.Priority?.Name ?? "Unknown",
                ProjectName = task.Project?.ProjectName ?? "Unknown",
                ProjectNumber = task.Project?.ProjectNumber ?? "Unknown",
                MachineName = task.Machine?.Name,
                MachineId = task.MachineId,
                AssignedOperators = task.TasksOperators?
                    .Select(to => to.Operator.FullName)
                    .ToList() ?? new List<string>(),
                SprintOrder = task.SprintOrder,
                IsInSprint = task.IsInSprint,
                PlannedStartDate = task.PlannedStartDate,
                PlannedEndDate = task.PlannedEndDate,
                ActualHours = 0, // ActualHours not available in current model
                CreatorName = task.Creator?.UserName ?? "Unknown",
                Comment = task.Comment,
                Attachment = task.Attachment,
                CanBeCompleted = true, // Will be calculated based on capacity
                StatusReason = string.Empty
            };
        }
    }
}
