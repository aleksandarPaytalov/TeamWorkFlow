using TeamWorkFlow.Core.Models.Task;
using TeamWorkFlow.Core.Models.Pager;

namespace TeamWorkFlow.Core.Models.Admin
{
	public class AssignedTasksServiceModel
	{
		public IEnumerable<TaskServiceModel> AllAssignedTasks { get; set; }
			= new List<TaskServiceModel>();
		public PagerServiceModel Pager { get; set; } = null!;
	}
}
