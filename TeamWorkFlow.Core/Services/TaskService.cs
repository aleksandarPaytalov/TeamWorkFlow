using System.Globalization;
using Microsoft.EntityFrameworkCore;
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
                    ProjectNumber = t.Project.ProjectNumber,
                    Name = t.Name,
                    Description = t.Description,
                    Deadline = t.DeadLine.ToString(Messages.DateFormat, CultureInfo.InvariantCulture),
                    Priority = t.Priority.Name,
                    Status = t.TaskStatus.Name,
                    StartDate = t.StartDate.ToString(Messages.DateFormat, CultureInfo.InvariantCulture)
                })
                .ToListAsync();
        }
    }
}
