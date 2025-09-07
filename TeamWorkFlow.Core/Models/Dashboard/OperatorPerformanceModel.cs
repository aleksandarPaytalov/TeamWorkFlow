using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Model for individual operator performance data and comparative analysis
    /// </summary>
    public class OperatorPerformanceModel
    {
        /// <summary>
        /// Operator identifier
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Operator name for display
        /// </summary>
        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Operator email
        /// </summary>
        [Display(Name = "Email")]
        public string OperatorEmail { get; set; } = string.Empty;

        /// <summary>
        /// Total number of tasks completed by this operator
        /// </summary>
        [Display(Name = "Tasks Completed")]
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Total number of tasks currently in progress
        /// </summary>
        [Display(Name = "Tasks In Progress")]
        public int TasksInProgress { get; set; }

        /// <summary>
        /// Average completion time for this operator's tasks (hours)
        /// </summary>
        [Display(Name = "Avg Completion Time")]
        public decimal AverageCompletionTimeHours { get; set; }

        /// <summary>
        /// Efficiency rating compared to estimates (0-100, higher is better)
        /// </summary>
        [Display(Name = "Efficiency Rating")]
        public decimal EfficiencyRating { get; set; }

        /// <summary>
        /// On-time completion rate for this operator (0-100)
        /// </summary>
        [Display(Name = "On-Time Rate")]
        public decimal OnTimeCompletionRate { get; set; }

        /// <summary>
        /// Average time overrun percentage for this operator
        /// </summary>
        [Display(Name = "Avg Overrun %")]
        public decimal AverageOverrunPercentage { get; set; }

        /// <summary>
        /// Total productive hours logged by this operator
        /// </summary>
        [Display(Name = "Total Hours")]
        public decimal TotalProductiveHours { get; set; }

        /// <summary>
        /// Rank among all operators (1 = best performer)
        /// </summary>
        [Display(Name = "Rank")]
        public int Rank { get; set; }

        /// <summary>
        /// Performance relative to team average (percentage)
        /// </summary>
        [Display(Name = "vs Team Avg")]
        public decimal RelativeToTeamAverage { get; set; }

        /// <summary>
        /// Performance trend over the reporting period
        /// </summary>
        [Display(Name = "Trend")]
        public decimal PerformanceTrend { get; set; }

        /// <summary>
        /// Reporting period for this performance data
        /// </summary>
        [Display(Name = "Period")]
        public string ReportingPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Specialization areas or task types this operator excels in
        /// </summary>
        public List<string> Specializations { get; set; } = new List<string>();

        /// <summary>
        /// Performance breakdown by time period
        /// </summary>
        public List<PerformancePeriodModel> PerformanceHistory { get; set; } = new List<PerformancePeriodModel>();

        /// <summary>
        /// Task completion distribution by priority
        /// </summary>
        public Dictionary<string, int> TasksByPriority { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Average session duration for this operator (minutes)
        /// </summary>
        [Display(Name = "Avg Session Duration")]
        public decimal AverageSessionDurationMinutes { get; set; }

        /// <summary>
        /// Number of work sessions completed
        /// </summary>
        [Display(Name = "Total Sessions")]
        public int TotalSessions { get; set; }

        /// <summary>
        /// Quality score based on rework and corrections needed
        /// </summary>
        [Display(Name = "Quality Score")]
        public decimal QualityScore { get; set; }

        /// <summary>
        /// Formatted display of average completion time
        /// </summary>
        [Display(Name = "Avg Time")]
        public string AverageCompletionTimeFormatted
        {
            get
            {
                var totalMinutes = (int)(AverageCompletionTimeHours * 60);
                var hours = totalMinutes / 60;
                var minutes = totalMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        /// <summary>
        /// Formatted display of efficiency rating
        /// </summary>
        [Display(Name = "Efficiency")]
        public string EfficiencyRatingFormatted => $"{EfficiencyRating:F1}%";

        /// <summary>
        /// Formatted display of on-time completion rate
        /// </summary>
        [Display(Name = "On-Time")]
        public string OnTimeRateFormatted => $"{OnTimeCompletionRate:F1}%";

        /// <summary>
        /// Formatted display of average overrun percentage
        /// </summary>
        [Display(Name = "Overrun")]
        public string AverageOverrunFormatted
        {
            get
            {
                var sign = AverageOverrunPercentage >= 0 ? "+" : "";
                return $"{sign}{AverageOverrunPercentage:F1}%";
            }
        }

        /// <summary>
        /// Formatted display of total productive hours
        /// </summary>
        [Display(Name = "Hours")]
        public string TotalHoursFormatted => $"{TotalProductiveHours:F1}h";

        /// <summary>
        /// Formatted display of relative performance
        /// </summary>
        [Display(Name = "vs Team")]
        public string RelativePerformanceFormatted
        {
            get
            {
                var sign = RelativeToTeamAverage >= 0 ? "+" : "";
                return $"{sign}{RelativeToTeamAverage:F1}%";
            }
        }

        /// <summary>
        /// Formatted display of performance trend
        /// </summary>
        [Display(Name = "Trend")]
        public string PerformanceTrendFormatted
        {
            get
            {
                var sign = PerformanceTrend >= 0 ? "+" : "";
                return $"{sign}{PerformanceTrend:F1}%";
            }
        }

        /// <summary>
        /// CSS class for efficiency rating display
        /// </summary>
        public string EfficiencyClass
        {
            get
            {
                if (EfficiencyRating >= 85) return "text-success fw-bold";
                if (EfficiencyRating >= 70) return "text-success";
                if (EfficiencyRating >= 55) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// CSS class for rank display
        /// </summary>
        public string RankClass
        {
            get
            {
                if (Rank == 1) return "badge bg-success";
                if (Rank <= 3) return "badge bg-primary";
                if (Rank <= 5) return "badge bg-info";
                return "badge bg-secondary";
            }
        }

        /// <summary>
        /// CSS class for performance trend display
        /// </summary>
        public string TrendClass
        {
            get
            {
                if (PerformanceTrend > 5) return "text-success fw-bold";
                if (PerformanceTrend > 0) return "text-success";
                if (PerformanceTrend > -5) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// Icon for performance trend
        /// </summary>
        public string TrendIcon
        {
            get
            {
                if (PerformanceTrend > 0) return "fas fa-arrow-up";
                if (PerformanceTrend < 0) return "fas fa-arrow-down";
                return "fas fa-minus";
            }
        }

        /// <summary>
        /// CSS class for relative performance display
        /// </summary>
        public string RelativePerformanceClass
        {
            get
            {
                if (RelativeToTeamAverage > 10) return "text-success fw-bold";
                if (RelativeToTeamAverage > 0) return "text-success";
                if (RelativeToTeamAverage > -10) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// Performance status description
        /// </summary>
        public string PerformanceStatus
        {
            get
            {
                if (EfficiencyRating >= 85) return "Excellent";
                if (EfficiencyRating >= 70) return "Good";
                if (EfficiencyRating >= 55) return "Average";
                return "Needs Improvement";
            }
        }

        /// <summary>
        /// Indicates if this operator is a top performer
        /// </summary>
        public bool IsTopPerformer => Rank <= 3 && EfficiencyRating >= 75;

        /// <summary>
        /// Indicates if this operator needs attention
        /// </summary>
        public bool NeedsAttention => EfficiencyRating < 55 || OnTimeCompletionRate < 60;

        /// <summary>
        /// Formatted display of average session duration
        /// </summary>
        public string AverageSessionDurationFormatted
        {
            get
            {
                var hours = (int)AverageSessionDurationMinutes / 60;
                var minutes = (int)AverageSessionDurationMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        /// <summary>
        /// Workload intensity indicator
        /// </summary>
        public string WorkloadIntensity
        {
            get
            {
                var tasksPerWeek = TasksCompleted / Math.Max(1, ReportingPeriod.Contains("week") ? 1 : 4);
                if (tasksPerWeek >= 10) return "High";
                if (tasksPerWeek >= 5) return "Medium";
                return "Low";
            }
        }

        /// <summary>
        /// CSS class for workload intensity
        /// </summary>
        public string WorkloadIntensityClass
        {
            get
            {
                return WorkloadIntensity switch
                {
                    "High" => "badge bg-danger",
                    "Medium" => "badge bg-warning",
                    "Low" => "badge bg-success",
                    _ => "badge bg-secondary"
                };
            }
        }
    }

    /// <summary>
    /// Model for operator performance in a specific time period
    /// </summary>
    public class PerformancePeriodModel
    {
        /// <summary>
        /// Period identifier (e.g., "2024-W01", "2024-01")
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Start date of the period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the period
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Tasks completed in this period
        /// </summary>
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Efficiency rating for this period
        /// </summary>
        public decimal EfficiencyRating { get; set; }

        /// <summary>
        /// On-time completion rate for this period
        /// </summary>
        public decimal OnTimeRate { get; set; }

        /// <summary>
        /// Total hours worked in this period
        /// </summary>
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Formatted period display
        /// </summary>
        public string PeriodFormatted => $"{StartDate:dd/MM} - {EndDate:dd/MM}";
    }
}
