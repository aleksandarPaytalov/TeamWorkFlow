namespace TeamWorkFlow.Core.Models.Summary
{
	public class SummaryServiceModel
	{
		public int TotalTasks { get; set; }
		public int FinishedTasks { get; set; }


		public int TotalProjects { get; set; }
		public int ProjectsInProduction { get; set; }


		public int TotalParts { get; set; }
		public int TotalApprovedParts { get; set; }

		public int TotalWorkers { get; set; }
		public int TotalAvailableWorkers { get; set; }

		public int TotalMachines { get; set; }
		public int TotalAvailableMachines { get; set; }
	}
}
