namespace TeamWorkFlow.Core.Models.Project
{
    public class ProjectTimeCalculationServiceModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectNumber { get; set; } = string.Empty;

        // Manual estimate (existing field)
        public int TotalHoursSpent { get; set; }

        // Task-based calculations
        public double CalculatedActualHours { get; set; }
        public int TotalPlannedHours { get; set; }
        public double TimeOverview { get; set; }
        public double CompletionPercentage { get; set; }

        // Task counts
        public int FinishedTasksCount { get; set; }
        public int InProgressTasksCount { get; set; }
        public int OpenTasksCount { get; set; }
        public int TotalTasksCount { get; set; }

        // Calculated properties for display
        public string FormattedCompletionPercentage => $"{CompletionPercentage:F1}%";
        
        public string TimeOverviewStatus => TimeOverview switch
        {
            > 0 => "Over Planned",
            < 0 => "Under Planned",
            0 => "On Target",
            _ => "Unknown"
        };

        public string TimeOverviewDescription => TimeOverview switch
        {
            > 0 => $"Project took {TimeOverview:F1}h more than planned",
            < 0 => $"Project completed {Math.Abs(TimeOverview):F1}h under planned time",
            0 => "Project time matches planned estimate exactly",
            _ => "Time overview unavailable"
        };

        public bool IsOnTrack => Math.Abs(TimeOverview) <= (TotalPlannedHours * 0.1); // Within 10% variance

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

        public string FormattedTotalHoursSpent => GetFormattedDurationInt(TotalHoursSpent);
        public string FormattedCalculatedActualHours => GetFormattedDuration(CalculatedActualHours);
        public string FormattedTotalPlannedHours => GetFormattedDurationInt(TotalPlannedHours);
    }
}
