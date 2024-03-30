using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TeamWorkFlow.Core.Constants;
using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Infrastructure.Data;

namespace TeamWorkFlow.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly TeamWorkFlowDbContext _context;

        public TaskService(TeamWorkFlowDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<TaskViewModel>> GetAllTasksAsync()
        {
            return await _context.Tasks
                .AsNoTracking()
                .Select(t => new TaskViewModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Status = t.TaskStatus.Name,
                    Priority = t.Priority.Name,
                    ProjectNumber = t.Project.ProjectNumber,
                    StartDate = t.StartDate.ToString(Messages.DateFormat, CultureInfo.InvariantCulture)
                })
                .ToListAsync();
        }

        public async Task<ICollection<StatusViewModel>> GetAllStatusesAsync()
        {
            return await _context.TaskStatusEnumerable
                .Select(s => new StatusViewModel()
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        public async Task<ICollection<PriorityViewModel>> GetAllPrioritiesAsync()
        {
            return await _context.Priorities
                .Select(p => new PriorityViewModel()
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();
        }

        public async Task AddNewTaskAsync(AddTaskViewModel model, string userId)
        {
            DateTime startDate;
            DateTime endDate;
            DateTime deadLine;

            var startDateIsValid = DateTime.TryParseExact(model.StartDate, Messages.DateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);

            var endDateIsValid = DateTime.TryParseExact(model.EndDate, Messages.DateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);

            var deadLineIsValid = DateTime.TryParseExact(model.Deadline, Messages.DateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out deadLine);
            
            

            if (startDateIsValid)
            {
                var task = new Infrastructure.Data.Models.Task()
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    DeadLine = deadLine,
                    CreatorId = userId,
                    Description = model.Description,
                    Name = model.Name,
                    PriorityId = model.PriorityId,
                    TaskStatusId = model.StatusId,
                    ProjectId = model.ProjectId
                };

                await _context.AddAsync(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TaskStatusExistsAsync(int statusId)
        {
            return await _context.TaskStatusEnumerable.AnyAsync(s => s.Id == statusId);
        }

        public async Task<bool> PriorityExistsAsync(int priorityId)
        {
            return await _context.Priorities.AnyAsync(p => p.Id == priorityId);
        }
    }
}
