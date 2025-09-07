using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Dashboard
{
    /// <summary>
    /// Model for task completion efficiency calculations and metrics
    /// </summary>
    public class EfficiencyMetricsModel
    {
        /// <summary>
        /// Percentage of tasks completed on time or early (0-100)
        /// </summary>
        [Display(Name = "On-Time Completion Rate")]
        public decimal OnTimeCompletionRate { get; set; }

        /// <summary>
        /// Average percentage by which tasks exceed their estimated time
        /// Positive = over estimate, Negative = under estimate
        /// </summary>
        [Display(Name = "Average Time Overrun %")]
        public decimal AverageTimeOverrunPercentage { get; set; }

        /// <summary>
        /// Total number of tasks completed in the analysis period
        /// </summary>
        [Display(Name = "Total Tasks Completed")]
        public int TotalTasksCompleted { get; set; }

        /// <summary>
        /// Number of tasks completed on time or early
        /// </summary>
        [Display(Name = "Tasks Completed On Time")]
        public int TasksCompletedOnTime { get; set; }

        /// <summary>
        /// Number of tasks that exceeded their estimated time
        /// </summary>
        [Display(Name = "Tasks Over Estimate")]
        public int TasksOverEstimate { get; set; }

        /// <summary>
        /// Number of tasks completed under their estimated time
        /// </summary>
        [Display(Name = "Tasks Under Estimate")]
        public int TasksUnderEstimate { get; set; }

        /// <summary>
        /// Average actual completion time in hours
        /// </summary>
        [Display(Name = "Average Actual Time (Hours)")]
        public decimal AverageActualTimeHours { get; set; }

        /// <summary>
        /// Average estimated time in hours
        /// </summary>
        [Display(Name = "Average Estimated Time (Hours)")]
        public decimal AverageEstimatedTimeHours { get; set; }

        /// <summary>
        /// Efficiency trend over time (positive = improving, negative = declining)
        /// </summary>
        [Display(Name = "Efficiency Trend")]
        public decimal EfficiencyTrend { get; set; }

        /// <summary>
        /// Time period for trend calculation
        /// </summary>
        [Display(Name = "Trend Period")]
        public string TrendPeriod { get; set; } = string.Empty;

        /// <summary>
        /// Collection of efficiency data points for trend visualization
        /// </summary>
        public List<EfficiencyTrendPoint> TrendData { get; set; } = new List<EfficiencyTrendPoint>();

        /// <summary>
        /// Percentage of tasks with significant time variance (>20%)
        /// </summary>
        [Display(Name = "High Variance Tasks %")]
        public decimal HighVarianceTasksPercentage { get; set; }

        /// <summary>
        /// Most common reasons for delays (if available)
        /// </summary>
        public List<DelayReasonModel> CommonDelayReasons { get; set; } = new List<DelayReasonModel>();

        /// <summary>
        /// Formatted display of on-time completion rate
        /// </summary>
        [Display(Name = "On-Time Rate")]
        public string OnTimeCompletionRateFormatted => $"{OnTimeCompletionRate:F1}%";

        /// <summary>
        /// Formatted display of average time overrun
        /// </summary>
        [Display(Name = "Avg Overrun")]
        public string AverageTimeOverrunFormatted
        {
            get
            {
                var sign = AverageTimeOverrunPercentage >= 0 ? "+" : "";
                return $"{sign}{AverageTimeOverrunPercentage:F1}%";
            }
        }

        /// <summary>
        /// Formatted display of efficiency trend
        /// </summary>
        [Display(Name = "Trend")]
        public string EfficiencyTrendFormatted
        {
            get
            {
                var sign = EfficiencyTrend >= 0 ? "+" : "";
                return $"{sign}{EfficiencyTrend:F1}%";
            }
        }

        /// <summary>
        /// CSS class for on-time completion rate display
        /// </summary>
        public string OnTimeRateClass
        {
            get
            {
                if (OnTimeCompletionRate >= 90) return "text-success fw-bold";
                if (OnTimeCompletionRate >= 75) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// CSS class for time overrun display
        /// </summary>
        public string OverrunClass
        {
            get
            {
                if (Math.Abs(AverageTimeOverrunPercentage) <= 10) return "text-success";
                if (Math.Abs(AverageTimeOverrunPercentage) <= 25) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// CSS class for efficiency trend display
        /// </summary>
        public string TrendClass
        {
            get
            {
                if (EfficiencyTrend > 5) return "text-success fw-bold";
                if (EfficiencyTrend > 0) return "text-success";
                if (EfficiencyTrend > -5) return "text-warning";
                return "text-danger";
            }
        }

        /// <summary>
        /// Icon for efficiency trend display
        /// </summary>
        public string TrendIcon
        {
            get
            {
                if (EfficiencyTrend > 0) return "fas fa-arrow-up";
                if (EfficiencyTrend < 0) return "fas fa-arrow-down";
                return "fas fa-minus";
            }
        }

        /// <summary>
        /// Overall efficiency score (0-100)
        /// </summary>
        public decimal OverallEfficiencyScore
        {
            get
            {
                if (TotalTasksCompleted == 0) return 0;

                // Weight on-time completion more heavily
                var onTimeWeight = 0.6m;
                var overrunWeight = 0.4m;

                var onTimeScore = OnTimeCompletionRate;
                var overrunScore = Math.Max(0, 100 - Math.Abs(AverageTimeOverrunPercentage));

                return (onTimeScore * onTimeWeight) + (overrunScore * overrunWeight);
            }
        }

        /// <summary>
        /// Efficiency score status text
        /// </summary>
        public string EfficiencyStatus
        {
            get
            {
                if (OverallEfficiencyScore >= 85) return "Excellent";
                if (OverallEfficiencyScore >= 70) return "Good";
                if (OverallEfficiencyScore >= 55) return "Fair";
                return "Needs Improvement";
            }
        }

        /// <summary>
        /// Indicates if there's sufficient data for reliable metrics
        /// </summary>
        public bool HasSufficientData => TotalTasksCompleted >= 5;

        /// <summary>
        /// Formatted display of average actual time
        /// </summary>
        public string AverageActualTimeFormatted
        {
            get
            {
                var totalMinutes = (int)(AverageActualTimeHours * 60);
                var hours = totalMinutes / 60;
                var minutes = totalMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        /// <summary>
        /// Formatted display of average estimated time
        /// </summary>
        public string AverageEstimatedTimeFormatted
        {
            get
            {
                var totalMinutes = (int)(AverageEstimatedTimeHours * 60);
                var hours = totalMinutes / 60;
                var minutes = totalMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }
    }

    /// <summary>
    /// Model for efficiency trend data points
    /// </summary>
    public class EfficiencyTrendPoint
    {
        /// <summary>
        /// Date for this trend point
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// On-time completion rate for this period
        /// </summary>
        public decimal OnTimeRate { get; set; }

        /// <summary>
        /// Average overrun percentage for this period
        /// </summary>
        public decimal OverrunPercentage { get; set; }

        /// <summary>
        /// Number of tasks completed in this period
        /// </summary>
        public int TasksCompleted { get; set; }

        /// <summary>
        /// Formatted date for display
        /// </summary>
        public string DateFormatted => Date.ToString("dd/MM/yyyy");

        /// <summary>
        /// Period label for charts
        /// </summary>
        public string PeriodLabel { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model for common delay reasons
    /// </summary>
    public class DelayReasonModel
    {
        /// <summary>
        /// Reason category or description
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Number of occurrences
        /// </summary>
        public int Occurrences { get; set; }

        /// <summary>
        /// Percentage of total delays
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// Average delay time for this reason (minutes)
        /// </summary>
        public int AverageDelayMinutes { get; set; }

        /// <summary>
        /// Formatted percentage display
        /// </summary>
        public string PercentageFormatted => $"{Percentage:F1}%";

        /// <summary>
        /// Formatted average delay display
        /// </summary>
        public string AverageDelayFormatted
        {
            get
            {
                var hours = AverageDelayMinutes / 60;
                var minutes = AverageDelayMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }
    }
}
