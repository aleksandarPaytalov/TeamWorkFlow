namespace TeamWorkFlow.Core.Models.Machine
{
	public class MachineDetailsServiceModel : MachineServiceModel
	{
		public string ImageUrl { get; set; } = string.Empty;

		public double TotalMachineLoad { get; set; }
		
		public string? MaintenanceScheduleStartDate { get; set; }

		public string? MaintenanceScheduleEndDate { get; set; }
	}
}
