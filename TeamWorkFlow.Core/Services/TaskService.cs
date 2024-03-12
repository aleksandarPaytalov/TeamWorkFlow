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
                    Name = t.Name,
                    Description = t.Description,
                    Status = t.TaskStatus.Name,
                    Priority = t.Priority.Name,
                    ProjectNumber = t.Project.ProjectNumber,
                    StartDate = t.StartDate.ToString(Messages.DateFormat, CultureInfo.InvariantCulture)
                })
                .ToListAsync();
        }
    }
}
