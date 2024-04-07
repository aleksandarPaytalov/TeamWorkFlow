namespace TeamWorkFlow.Core.Models.Task
{
	public class TaskDeleteServiceModel
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Creator { get; set; } = string.Empty;
	}
}
