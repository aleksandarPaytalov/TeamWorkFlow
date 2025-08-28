using TeamWorkFlow.Core.Models.Part;

namespace TeamWorkFlow.Core.Models.Project
{
	public class ProjectDetailsServiceModel : ProjectServiceModel
	{
		public string ClientName { get; set; } = string.Empty;

		public string Appliance { get; set; } = string.Empty;

		public int TotalHoursSpent { get; set; }

		public IEnumerable<ProjectPartServiceModel> Parts { get; set; } = new List<ProjectPartServiceModel>();

		// Task-based time calculations
		public int CalculatedTotalHours { get; set; }
		public int TotalEstimatedHours { get; set; }
		public int TimeVariance { get; set; }
		public double CompletionPercentage { get; set; }

		// Task counts
		public int FinishedTasksCount { get; set; }
		public int InProgressTasksCount { get; set; }
		public int OpenTasksCount { get; set; }
		public int TotalTasksCount { get; set; }

		// Calculated properties for display
		public string FormattedCompletionPercentage => $"{CompletionPercentage:F1}%";

		public string TimeVarianceStatus => TimeVariance switch
		{
			> 0 => "Under Estimate",
			< 0 => "Over Estimate",
			0 => "On Target"
		};

		public bool IsOnTrack => Math.Abs(TimeVariance) <= (TotalHoursSpent * 0.1); // Within 10% variance

		public string ProjectHealthStatus
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

		public string GetFormattedDuration(int hours)
		{
			if (hours < 8)
				return $"{hours}h";

			int days = hours / 8;
			int remainingHours = hours % 8;

			if (remainingHours == 0)
				return $"{days}d";

			return $"{days}d {remainingHours}h";
		}

		public string FormattedTotalHoursSpent => GetFormattedDuration(TotalHoursSpent);
		public string FormattedCalculatedTotalHours => GetFormattedDuration(CalculatedTotalHours);
		public string FormattedTotalEstimatedHours => GetFormattedDuration(TotalEstimatedHours);
	}
}
