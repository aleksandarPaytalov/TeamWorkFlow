using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Pager;

namespace TeamWorkFlow.Core.Models.Task
{
    public class TaskServiceModel : ITaskModel
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
		public string Priority { get; set; } = string.Empty;
		public string? Deadline { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }
    }

    public class PaginatedTasksViewModel
    {
        public IEnumerable<TaskServiceModel> Tasks { get; set; } = new List<TaskServiceModel>();
        public PagerServiceModel Pager { get; set; }
    }
}
