using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Pager;

namespace TeamWorkFlow.Core.Models.Project
{
	public class ProjectServiceModel : IProjectModel
	{
		public int Id { get; set; }

		public string ProjectName { get; set; } = string.Empty;

		public string ProjectNumber { get; set; } = string.Empty;

		public string Status { get; set; } = string.Empty;

		public int TotalParts { get; set; }

		// Time tracking properties
		public int CalculatedTotalHours { get; set; }
		public int TotalEstimatedHours { get; set; }
		public double CompletionPercentage { get; set; }

		// Calculated properties for display
		public string FormattedCompletionPercentage => $"{CompletionPercentage:F1}%";

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

		public string FormattedCalculatedTotalHours => GetFormattedDuration(CalculatedTotalHours);
		public string FormattedTotalEstimatedHours => GetFormattedDuration(TotalEstimatedHours);

		// Progress bar color logic
		public string ProgressBarColor
		{
			get
			{
				if (CompletionPercentage >= 100) return "success";
				if (CompletionPercentage >= 75) return "warning";
				return "primary";
			}
		}
	}

	public class PaginatedProjectsViewModel
	{
		public IEnumerable<ProjectServiceModel> Projects { get; set; } = new List<ProjectServiceModel>();
		public PagerServiceModel Pager { get; set; } = null!;
	}
}
