using TeamWorkFlow.Core.Models.Part;

namespace TeamWorkFlow.Core.Models.Project
{
	public class ProjectDetailsServiceModel : ProjectServiceModel
	{
		public string ClientName { get; set; } = string.Empty;

		public string Appliance { get; set; } = string.Empty;

		public IEnumerable<ProjectPartServiceModel> Parts { get; set; } = new List<ProjectPartServiceModel>();

		// Operator time contributions
		public IEnumerable<OperatorContributionServiceModel> OperatorContributions { get; set; } = new List<OperatorContributionServiceModel>();

		// Task-based time calculations
		public new int CalculatedTotalHours { get; set; }
		public new int TotalEstimatedHours { get; set; }
		public new double CompletionPercentage { get; set; }

		// Cost calculation properties
		public decimal HourlyRate { get; set; }
		public decimal TotalLaborCost => CalculatedTotalHours * HourlyRate;

		// Task counts
		public int FinishedTasksCount { get; set; }
		public int InProgressTasksCount { get; set; }
		public int OpenTasksCount { get; set; }
		public int TotalTasksCount { get; set; }

		// Calculated properties for display
		public new string FormattedCompletionPercentage => $"{CompletionPercentage:F1}%";

		public bool IsOnTrack => CompletionPercentage >= 90; // Consider on track if 90%+ complete

		public new string ProjectHealthStatus
		{
			get
			{
				if (CompletionPercentage >= 100) return "Completed";
				if (CompletionPercentage >= 75) return "Near Completion";
				if (CompletionPercentage >= 50) return "On Track";
				if (CompletionPercentage >= 25) return "In Progress";
				return "Just Started";
			}
		}

		public new string GetFormattedDuration(int hours)
		{
			if (hours < 8)
				return $"{hours}h";

			int days = hours / 8;
			int remainingHours = hours % 8;

			if (remainingHours == 0)
				return $"{days}d";

			return $"{days}d {remainingHours}h";
		}

		public new string FormattedCalculatedTotalHours => GetFormattedDuration(CalculatedTotalHours);
		public new string FormattedTotalEstimatedHours => GetFormattedDuration(TotalEstimatedHours);
	}
}
