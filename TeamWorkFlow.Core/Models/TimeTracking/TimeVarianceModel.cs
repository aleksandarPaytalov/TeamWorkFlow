using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.TimeTracking
{
    /// <summary>
    /// View model for variance analysis between estimated and actual time
    /// </summary>
    public class TimeVarianceModel
    {
        /// <summary>
        /// Task identifier
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Task name for display purposes
        /// </summary>
        [Display(Name = "Task")]
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Operator identifier
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Operator name for display purposes
        /// </summary>
        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Originally estimated time in hours
        /// </summary>
        [Display(Name = "Estimated Time (Hours)")]
        public decimal EstimatedTimeHours { get; set; }

        /// <summary>
        /// Total actual time spent in minutes
        /// </summary>
        [Display(Name = "Actual Time (Minutes)")]
        public int ActualTimeMinutes { get; set; }

        /// <summary>
        /// Variance in minutes (positive = over estimate, negative = under estimate)
        /// </summary>
        [Display(Name = "Variance (Minutes)")]
        public int VarianceMinutes { get; set; }

        /// <summary>
        /// Variance as percentage of estimated time
        /// </summary>
        [Display(Name = "Variance %")]
        public decimal VariancePercentage { get; set; }

        /// <summary>
        /// Number of work sessions completed
        /// </summary>
        [Display(Name = "Sessions")]
        public int TotalSessions { get; set; }

        /// <summary>
        /// Average session duration in minutes
        /// </summary>
        [Display(Name = "Avg Session (Minutes)")]
        public decimal AverageSessionMinutes { get; set; }

        /// <summary>
        /// Date when task was started
        /// </summary>
        [Display(Name = "Started")]
        public DateTime? TaskStartDate { get; set; }

        /// <summary>
        /// Date when task was completed (if finished)
        /// </summary>
        [Display(Name = "Completed")]
        public DateTime? TaskCompletedDate { get; set; }

        /// <summary>
        /// Indicates if task is completed
        /// </summary>
        public bool IsTaskCompleted => TaskCompletedDate.HasValue;

        /// <summary>
        /// Indicates if actual time is over the estimate
        /// </summary>
        public bool IsOverEstimate => VarianceMinutes > 0;

        /// <summary>
        /// Indicates if actual time is under the estimate
        /// </summary>
        public bool IsUnderEstimate => VarianceMinutes < 0;

        /// <summary>
        /// Indicates if variance is within acceptable range (±10%)
        /// </summary>
        public bool IsWithinAcceptableRange => Math.Abs(VariancePercentage) <= 10;

        /// <summary>
        /// Indicates if variance is significant (>20%)
        /// </summary>
        public bool IsSignificantVariance => Math.Abs(VariancePercentage) > 20;

        /// <summary>
        /// Indicates if variance is critical (>50%)
        /// </summary>
        public bool IsCriticalVariance => Math.Abs(VariancePercentage) > 50;

        /// <summary>
        /// Formatted display of estimated time
        /// </summary>
        [Display(Name = "Estimated")]
        public string EstimatedTimeFormatted => FormatTimeFromHours(EstimatedTimeHours);

        /// <summary>
        /// Formatted display of actual time
        /// </summary>
        [Display(Name = "Actual")]
        public string ActualTimeFormatted => FormatTimeFromMinutes(ActualTimeMinutes);

        /// <summary>
        /// Formatted display of variance with sign
        /// </summary>
        [Display(Name = "Variance")]
        public string VarianceFormatted
        {
            get
            {
                var sign = VarianceMinutes >= 0 ? "+" : "";
                return $"{sign}{FormatTimeFromMinutes(Math.Abs(VarianceMinutes))}";
            }
        }

        /// <summary>
        /// Formatted display of variance percentage with sign
        /// </summary>
        [Display(Name = "Variance %")]
        public string VariancePercentageFormatted
        {
            get
            {
                var sign = VariancePercentage >= 0 ? "+" : "";
                return $"{sign}{VariancePercentage:F1}%";
            }
        }

        /// <summary>
        /// Formatted display of average session duration
        /// </summary>
        [Display(Name = "Avg Session")]
        public string AverageSessionFormatted => FormatTimeFromMinutes((int)AverageSessionMinutes);

        /// <summary>
        /// CSS class for variance display based on severity
        /// </summary>
        public string VarianceClass
        {
            get
            {
                if (IsCriticalVariance)
                    return IsOverEstimate ? "text-danger fw-bold" : "text-primary fw-bold";
                if (IsSignificantVariance)
                    return IsOverEstimate ? "text-warning fw-bold" : "text-info fw-bold";
                if (IsWithinAcceptableRange)
                    return "text-success";
                return IsOverEstimate ? "text-warning" : "text-info";
            }
        }

        /// <summary>
        /// Status indicator text for variance
        /// </summary>
        public string VarianceStatus
        {
            get
            {
                if (IsWithinAcceptableRange) return "On Track";
                if (IsCriticalVariance) return IsOverEstimate ? "Critical Overrun" : "Significant Underestimate";
                if (IsSignificantVariance) return IsOverEstimate ? "Significant Overrun" : "Notable Underestimate";
                return IsOverEstimate ? "Over Estimate" : "Under Estimate";
            }
        }

        /// <summary>
        /// Badge CSS class for status display
        /// </summary>
        public string StatusBadgeClass
        {
            get
            {
                if (IsWithinAcceptableRange) return "badge bg-success";
                if (IsCriticalVariance) return "badge bg-danger";
                if (IsSignificantVariance) return "badge bg-warning";
                return IsOverEstimate ? "badge bg-warning" : "badge bg-info";
            }
        }

        /// <summary>
        /// Formatted display of task start date
        /// </summary>
        public string TaskStartDateFormatted => TaskStartDate?.ToString("dd/MM/yyyy") ?? "Not Started";

        /// <summary>
        /// Formatted display of task completion date
        /// </summary>
        public string TaskCompletedDateFormatted => TaskCompletedDate?.ToString("dd/MM/yyyy") ?? "In Progress";

        /// <summary>
        /// Helper method to format time from hours to readable format
        /// </summary>
        private static string FormatTimeFromHours(decimal hours)
        {
            var totalMinutes = (int)(hours * 60);
            return FormatTimeFromMinutes(totalMinutes);
        }

        /// <summary>
        /// Helper method to format time from minutes to readable format
        /// </summary>
        private static string FormatTimeFromMinutes(int totalMinutes)
        {
            if (totalMinutes <= 0) return "0m";

            var hours = totalMinutes / 60;
            var minutes = totalMinutes % 60;

            if (hours > 0 && minutes > 0)
                return $"{hours}h {minutes}m";
            else if (hours > 0)
                return $"{hours}h";
            else
                return $"{minutes}m";
        }
    }
}
