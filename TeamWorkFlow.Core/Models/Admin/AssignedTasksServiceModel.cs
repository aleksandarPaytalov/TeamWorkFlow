using TeamWorkFlow.Core.Models.Task;

namespace TeamWorkFlow.Core.Models.Admin
{
	public class AssignedTasksServiceModel
	{
		public IEnumerable<TaskServiceModel> AllAssignedTasks { get; set; }
			= new List<TaskServiceModel>();
	}
}
