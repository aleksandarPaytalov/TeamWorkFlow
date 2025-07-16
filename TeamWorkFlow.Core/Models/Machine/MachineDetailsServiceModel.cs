namespace TeamWorkFlow.Core.Models.Machine
{
	public class MachineDetailsServiceModel : MachineServiceModel
	{
		public string ImageUrl { get; set; } = string.Empty;

		public double TotalMachineLoad { get; set; }

		public string? MaintenanceScheduleStartDate { get; set; }

		public string? MaintenanceScheduleEndDate { get; set; }

		// Additional assignment details for details page
		public string? AssignedTaskDescription { get; set; }
		public string? AssignedTaskDeadline { get; set; }
		public string? AssignedTaskPriority { get; set; }
		public ICollection<AssignedOperatorInfo> AssignedOperators { get; set; } = new List<AssignedOperatorInfo>();
	}

	public class AssignedOperatorInfo
	{
		public int OperatorId { get; set; }
		public string OperatorName { get; set; } = string.Empty;
		public string OperatorEmail { get; set; } = string.Empty;
	}
}
