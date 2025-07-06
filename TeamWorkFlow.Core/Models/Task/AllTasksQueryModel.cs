using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Core.Enumerations;

namespace TeamWorkFlow.Core.Models.Task
{
    public class AllTasksQueryModel
    {
        public int TasksPerPage { get; } = 10;

        [Display(Name = "Search by task name or project number")]
        public string Search { get; init; } = null!;

        public TaskSorting Sorting { get; init; }
        
        public int CurrentPage { get; init; } = 1;

        public int TotalTasksCount { get; set; }

        public IEnumerable<TaskServiceModel> Tasks { get; set; } = new List<TaskServiceModel>();
    }
}
