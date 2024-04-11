using TeamWorkFlow.Core.Contracts;

namespace TeamWorkFlow.Core.Models.Task
{
	public class TaskDeleteServiceModel : ITaskModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Creator { get; set; } = string.Empty;
	}
}
