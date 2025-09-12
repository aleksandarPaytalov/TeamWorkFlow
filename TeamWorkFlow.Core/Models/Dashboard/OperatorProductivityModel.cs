using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Model for detailed productivity metrics for individual operators
    /// </summary>
    public class OperatorProductivityModel
    {
        /// <summary>
        /// Operator identifier
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Operator name
        /// </summary>
        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Operator email
        /// </summary>
        [Display(Name = "Email")]
        public string OperatorEmail { get; set; } = string.Empty;

        /// <summary>
        /// Analysis period start date
        /// </summary>
        [Display(Name = "Period Start")]
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// Analysis period end date
        /// </summary>
        [Display(Name = "Period End")]
        public DateTime PeriodEnd { get; set; }

        /// <summary>
        /// Total tasks completed in the period
        /// </summary>
        [Display(Name = "Tasks Completed")]
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Total tasks started but not completed
        /// </summary>
        [Display(Name = "Tasks In Progress")]
        public int TasksInProgress { get; set; }

        /// <summary>
        /// Total productive hours logged
        /// </summary>
        [Display(Name = "Productive Hours")]
        public decimal TotalProductiveHours { get; set; }

        /// <summary>
        /// Average hours per completed task
        /// </summary>
        [Display(Name = "Avg Hours per Task")]
        public decimal AverageHoursPerTask { get; set; }

        /// <summary>
        /// Tasks completed per day (average)
        /// </summary>
        [Display(Name = "Tasks per Day")]
        public decimal TasksPerDay { get; set; }

        /// <summary>
        /// Efficiency rating (0-100)
        /// </summary>
        [Display(Name = "Efficiency Rating")]
        public decimal EfficiencyRating { get; set; }

        /// <summary>
        /// Quality score based on rework and corrections (0-100)
        /// </summary>
        [Display(Name = "Quality Score")]
        public decimal QualityScore { get; set; }

        /// <summary>
        /// On-time delivery rate (0-100)
        /// </summary>
        [Display(Name = "On-Time Rate")]
        public decimal OnTimeDeliveryRate { get; set; }

        /// <summary>
        /// Average time variance percentage
        /// </summary>
        [Display(Name = "Time Variance")]
        public decimal AverageTimeVariance { get; set; }

        /// <summary>
        /// Total number of work sessions
        /// </summary>
        [Display(Name = "Work Sessions")]
        public int TotalWorkSessions { get; set; }

        /// <summary>
        /// Average session duration in minutes
        /// </summary>
        [Display(Name = "Avg Session Duration")]
        public decimal AverageSessionDurationMinutes { get; set; }

        /// <summary>
        /// Focus score based on session patterns (0-100)
        /// </summary>
        [Display(Name = "Focus Score")]
        public decimal FocusScore { get; set; }

        /// <summary>
        /// Consistency score based on daily performance variation (0-100)
        /// </summary>
        [Display(Name = "Consistency Score")]
        public decimal ConsistencyScore { get; set; }

        /// <summary>
        /// Productivity breakdown by task type
        /// </summary>
        public List<TaskTypeProductivityModel> ProductivityByTaskType { get; set; } = new List<TaskTypeProductivityModel>();

        /// <summary>
        /// Daily productivity data for the period
        /// </summary>
        public List<DailyProductivityModel> DailyProductivity { get; set; } = new List<DailyProductivityModel>();

        /// <summary>
        /// Productivity trends over time
        /// </summary>
        public List<ProductivityTrendModel> ProductivityTrends { get; set; } = new List<ProductivityTrendModel>();

        /// <summary>
        /// Peak performance hours analysis
        /// </summary>
        public List<HourlyProductivityModel> HourlyProductivity { get; set; } = new List<HourlyProductivityModel>();

        /// <summary>
        /// Collaboration metrics
        /// </summary>
        public CollaborationMetricsModel CollaborationMetrics { get; set; } = new CollaborationMetricsModel();

        /// <summary>
        /// Improvement recommendations
        /// </summary>
        public List<ProductivityRecommendationModel> Recommendations { get; set; } = new List<ProductivityRecommendationModel>();

        /// <summary>
        /// Formatted period display
        /// </summary>
        [Display(Name = "Analysis Period")]
        public string PeriodFormatted => $"{PeriodStart:dd/MM/yyyy} - {PeriodEnd:dd/MM/yyyy}";

        /// <summary>
        /// Formatted total productive hours
        /// </summary>
        [Display(Name = "Total Hours")]
        public string TotalProductiveHoursFormatted => $"{TotalProductiveHours:F1}h";

        /// <summary>
        /// Formatted average hours per task
        /// </summary>
        [Display(Name = "Avg Hours/Task")]
        public string AverageHoursPerTaskFormatted => $"{AverageHoursPerTask:F1}h";

        /// <summary>
        /// Formatted tasks per day
        /// </summary>
        [Display(Name = "Tasks/Day")]
        public string TasksPerDayFormatted => $"{TasksPerDay:F1}";

        /// <summary>
        /// Formatted efficiency rating
        /// </summary>
        [Display(Name = "Efficiency")]
        public string EfficiencyRatingFormatted => $"{EfficiencyRating:F1}%";

        /// <summary>
        /// Formatted quality score
        /// </summary>
        [Display(Name = "Quality")]
        public string QualityScoreFormatted => $"{QualityScore:F1}%";

        /// <summary>
        /// Formatted on-time delivery rate
        /// </summary>
        [Display(Name = "On-Time")]
        public string OnTimeDeliveryRateFormatted => $"{OnTimeDeliveryRate:F1}%";

        /// <summary>
        /// Formatted time variance
        /// </summary>
        [Display(Name = "Variance")]
        public string AverageTimeVarianceFormatted
        {
            get
            {
                var sign = AverageTimeVariance >= 0 ? "+" : "";
                return $"{sign}{AverageTimeVariance:F1}%";
            }
        }

        /// <summary>
        /// Formatted average session duration
        /// </summary>
        [Display(Name = "Avg Session")]
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
        /// Overall productivity score (0-100)
        /// </summary>
        public decimal OverallProductivityScore
        {
            get
            {
                // Weighted average of key metrics
                var weights = new Dictionary<decimal, decimal>
                {
                    { EfficiencyRating, 0.25m },
                    { QualityScore, 0.20m },
                    { OnTimeDeliveryRate, 0.20m },
                    { FocusScore, 0.15m },
                    { ConsistencyScore, 0.20m }
                };

                return weights.Sum(w => w.Key * w.Value);
            }
        }

        /// <summary>
        /// Productivity status description
        /// </summary>
        public string ProductivityStatus
        {
            get
            {
                if (OverallProductivityScore >= 85) return "Excellent";
                if (OverallProductivityScore >= 70) return "Good";
                if (OverallProductivityScore >= 55) return "Average";
                if (OverallProductivityScore >= 40) return "Below Average";
                return "Needs Improvement";
            }
        }

        /// <summary>
        /// CSS class for productivity status
        /// </summary>
        public string ProductivityStatusClass
        {
            get
            {
                if (OverallProductivityScore >= 85) return "text-success fw-bold";
                if (OverallProductivityScore >= 70) return "text-success";
                if (OverallProductivityScore >= 55) return "text-warning";
                if (OverallProductivityScore >= 40) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// Workload intensity indicator
        /// </summary>
        public string WorkloadIntensity
        {
            get
            {
                if (TasksPerDay >= 3) return "High";
                if (TasksPerDay >= 1.5m) return "Medium";
                if (TasksPerDay >= 0.5m) return "Low";
                return "Very Low";
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
                    "Very Low" => "badge bg-info",
                    _ => "badge bg-secondary"
                };
            }
        }

        /// <summary>
        /// Indicates if operator is performing above average
        /// </summary>
        public bool IsAboveAverage => OverallProductivityScore >= 70;

        /// <summary>
        /// Indicates if operator needs attention
        /// </summary>
        public bool NeedsAttention => OverallProductivityScore < 55 || OnTimeDeliveryRate < 60;

        /// <summary>
        /// Top strength area for this operator
        /// </summary>
        public string TopStrength
        {
            get
            {
                var scores = new Dictionary<string, decimal>
                {
                    { "Efficiency", EfficiencyRating },
                    { "Quality", QualityScore },
                    { "Timeliness", OnTimeDeliveryRate },
                    { "Focus", FocusScore },
                    { "Consistency", ConsistencyScore }
                };

                return scores.OrderByDescending(s => s.Value).First().Key;
            }
        }

        /// <summary>
        /// Area needing most improvement
        /// </summary>
        public string ImprovementArea
        {
            get
            {
                var scores = new Dictionary<string, decimal>
                {
                    { "Efficiency", EfficiencyRating },
                    { "Quality", QualityScore },
                    { "Timeliness", OnTimeDeliveryRate },
                    { "Focus", FocusScore },
                    { "Consistency", ConsistencyScore }
                };

                return scores.OrderBy(s => s.Value).First().Key;
            }
        }
    }

    /// <summary>
    /// Model for productivity by task type
    /// </summary>
    public class TaskTypeProductivityModel
    {
        /// <summary>
        /// Task type or category
        /// </summary>
        public string TaskType { get; set; } = string.Empty;

        /// <summary>
        /// Number of tasks of this type completed
        /// </summary>
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Average completion time for this task type
        /// </summary>
        public decimal AverageCompletionHours { get; set; }

        /// <summary>
        /// Efficiency rating for this task type
        /// </summary>
        public decimal EfficiencyRating { get; set; }

        /// <summary>
        /// Percentage of total tasks
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// Formatted percentage display
        /// </summary>
        public string PercentageFormatted => $"{Percentage:F1}%";

        /// <summary>
        /// Formatted average completion time
        /// </summary>
        public string AverageCompletionFormatted => $"{AverageCompletionHours:F1}h";
    }

    /// <summary>
    /// Model for daily productivity data
    /// </summary>
    public class DailyProductivityModel
    {
        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Tasks completed on this date
        /// </summary>
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Hours worked on this date
        /// </summary>
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Efficiency rating for this date
        /// </summary>
        public decimal EfficiencyRating { get; set; }

        /// <summary>
        /// Number of work sessions
        /// </summary>
        public int WorkSessions { get; set; }

        /// <summary>
        /// Formatted date display
        /// </summary>
        public string DateFormatted => Date.ToString("dd/MM/yyyy");

        /// <summary>
        /// Day of week
        /// </summary>
        public string DayOfWeek => Date.ToString("dddd");
    }

    /// <summary>
    /// Model for productivity trends
    /// </summary>
    public class ProductivityTrendModel
    {
        /// <summary>
        /// Period identifier
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// Start date of period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of period
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Productivity score for this period
        /// </summary>
        public decimal ProductivityScore { get; set; }

        /// <summary>
        /// Tasks completed in this period
        /// </summary>
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Hours worked in this period
        /// </summary>
        public decimal HoursWorked { get; set; }

        /// <summary>
        /// Trend direction compared to previous period
        /// </summary>
        public string TrendDirection { get; set; } = "Stable";

        /// <summary>
        /// Percentage change from previous period
        /// </summary>
        public decimal ChangePercentage { get; set; }
    }

    /// <summary>
    /// Model for hourly productivity analysis
    /// </summary>
    public class HourlyProductivityModel
    {
        /// <summary>
        /// Hour of day (0-23)
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Average productivity score for this hour
        /// </summary>
        public decimal ProductivityScore { get; set; }

        /// <summary>
        /// Number of work sessions during this hour
        /// </summary>
        public int SessionCount { get; set; }

        /// <summary>
        /// Average session duration for this hour
        /// </summary>
        public decimal AverageSessionMinutes { get; set; }

        /// <summary>
        /// Formatted hour display
        /// </summary>
        public string HourFormatted => $"{Hour:D2}:00";

        /// <summary>
        /// Time period description
        /// </summary>
        public string TimePeriod
        {
            get
            {
                if (Hour >= 6 && Hour < 12) return "Morning";
                if (Hour >= 12 && Hour < 18) return "Afternoon";
                if (Hour >= 18 && Hour < 22) return "Evening";
                return "Night";
            }
        }
    }

    /// <summary>
    /// Model for collaboration metrics
    /// </summary>
    public class CollaborationMetricsModel
    {
        /// <summary>
        /// Number of projects worked on
        /// </summary>
        public int ProjectsWorkedOn { get; set; }

        /// <summary>
        /// Number of different operators collaborated with
        /// </summary>
        public int CollaboratorsCount { get; set; }

        /// <summary>
        /// Average team size for projects
        /// </summary>
        public decimal AverageTeamSize { get; set; }

        /// <summary>
        /// Collaboration effectiveness score (0-100)
        /// </summary>
        public decimal CollaborationScore { get; set; }

        /// <summary>
        /// Knowledge sharing activities
        /// </summary>
        public int KnowledgeSharingActivities { get; set; }

        /// <summary>
        /// Cross-functional tasks completed
        /// </summary>
        public int CrossFunctionalTasks { get; set; }
    }

    /// <summary>
    /// Model for productivity recommendations
    /// </summary>
    public class ProductivityRecommendationModel
    {
        /// <summary>
        /// Recommendation title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Recommendation description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Category of recommendation
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Priority level (1-10)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Expected impact if implemented (1-10)
        /// </summary>
        public int ExpectedImpact { get; set; }

        /// <summary>
        /// Based on which metric or analysis
        /// </summary>
        public string BasedOn { get; set; } = string.Empty;

        /// <summary>
        /// Actionable steps
        /// </summary>
        public List<string> ActionableSteps { get; set; } = new List<string>();

        /// <summary>
        /// CSS class for priority display
        /// </summary>
        public string PriorityClass
        {
            get
            {
                if (Priority >= 8) return "badge bg-danger";
                if (Priority >= 6) return "badge bg-warning";
                if (Priority >= 4) return "badge bg-info";
                return "badge bg-secondary";
            }
        }
    }
}
