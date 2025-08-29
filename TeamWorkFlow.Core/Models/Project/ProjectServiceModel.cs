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
		public double CalculatedActualHours { get; set; }
		public int TotalPlannedHours { get; set; }
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

		public string GetFormattedDuration(double hours)
		{
			if (hours < 8)
				return $"{hours:F1}h";

			int days = (int)(hours / 8);
			double remainingHours = hours % 8;

			if (remainingHours < 0.1)
				return $"{days}d";

			return $"{days}d {remainingHours:F1}h";
		}

		public string GetFormattedDurationInt(int hours)
		{
			if (hours < 8)
				return $"{hours}h";

			int days = hours / 8;
			int remainingHours = hours % 8;

			if (remainingHours == 0)
				return $"{days}d";

			return $"{days}d {remainingHours}h";
		}

		public string FormattedCalculatedActualHours => GetFormattedDuration(CalculatedActualHours);
		public string FormattedTotalPlannedHours => GetFormattedDurationInt(TotalPlannedHours);

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
