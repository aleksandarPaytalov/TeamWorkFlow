namespace TeamWorkFlow.Core.Models.Task
{
	public class TaskDetailsServiceModel : TaskServiceModel
	{
		public string? AssignedMachineName { get; set; }
		public string Creator { get; set; } = string.Empty;
	}
}
