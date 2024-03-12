namespace TeamWorkFlow.Core.Models.Task
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
		public string Priority { get; set; } = string.Empty;
		public string Deadline { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }
        public string Creator { get; set; } = string.Empty;

    }
}
