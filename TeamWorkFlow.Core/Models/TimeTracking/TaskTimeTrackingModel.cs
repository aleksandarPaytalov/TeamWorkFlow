using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.TimeTracking
{
    /// <summary>
    /// View model for task time tracking information including session status, timing data, and completion metrics
    /// </summary>
    public class TaskTimeTrackingModel
    {
        /// <summary>
        /// Task identifier
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Task name/title
        /// </summary>
        [Display(Name = "Task Name")]
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Task description
        /// </summary>
        [Display(Name = "Description")]
        public string TaskDescription { get; set; } = string.Empty;

        /// <summary>
        /// Estimated time for task completion in hours
        /// </summary>
        [Display(Name = "Estimated Time (Hours)")]
        public decimal EstimatedTimeHours { get; set; }

        /// <summary>
        /// Current operator identifier
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Current operator name
        /// </summary>
        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Current operator email
        /// </summary>
        [Display(Name = "Operator Email")]
        public string OperatorEmail { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if there is an active work session for this task and operator
        /// </summary>
        [Display(Name = "Has Active Session")]
        public bool HasActiveSession { get; set; }

        /// <summary>
        /// Current session start time (if active session exists)
        /// </summary>
        [Display(Name = "Session Started")]
        public DateTime? CurrentSessionStartTime { get; set; }

        /// <summary>
        /// Indicates if current session is paused
        /// </summary>
        [Display(Name = "Session Paused")]
        public bool IsCurrentSessionPaused { get; set; }

        /// <summary>
        /// Total time paused in current session (minutes)
        /// </summary>
        [Display(Name = "Total Paused Time")]
        public int CurrentSessionPausedMinutes { get; set; }

        /// <summary>
        /// Current session status (Active, Paused, etc.)
        /// </summary>
        [Display(Name = "Session Status")]
        public string CurrentSessionStatus { get; set; } = string.Empty;

        /// <summary>
        /// Total actual time spent on task across all completed sessions (minutes)
        /// </summary>
        [Display(Name = "Total Actual Time")]
        public int TotalActualTimeMinutes { get; set; }

        /// <summary>
        /// Total number of completed work sessions
        /// </summary>
        [Display(Name = "Completed Sessions")]
        public int TotalCompletedSessions { get; set; }

        /// <summary>
        /// Collection of recent work sessions for this task
        /// </summary>
        public List<WorkSessionModel> RecentSessions { get; set; } = new List<WorkSessionModel>();

        /// <summary>
        /// Completion percentage based on actual vs estimated time
        /// </summary>
        [Display(Name = "Completion %")]
        public decimal CompletionPercentage { get; set; }

        /// <summary>
        /// Formatted display of estimated time (e.g., "2h 30m")
        /// </summary>
        [Display(Name = "Estimated Time")]
        public string EstimatedTimeFormatted => FormatTimeFromHours(EstimatedTimeHours);

        /// <summary>
        /// Formatted display of total actual time (e.g., "1h 45m")
        /// </summary>
        [Display(Name = "Actual Time")]
        public string TotalActualTimeFormatted => FormatTimeFromMinutes(TotalActualTimeMinutes);

        /// <summary>
        /// Formatted display of current session duration (if active)
        /// </summary>
        [Display(Name = "Current Session Duration")]
        public string CurrentSessionDurationFormatted
        {
            get
            {
                if (!HasActiveSession || !CurrentSessionStartTime.HasValue)
                    return "00:00";

                var elapsed = DateTime.UtcNow - CurrentSessionStartTime.Value;
                var totalMinutes = (int)elapsed.TotalMinutes - CurrentSessionPausedMinutes;
                return FormatTimeFromMinutes(Math.Max(0, totalMinutes));
            }
        }

        /// <summary>
        /// Indicates if task is over the estimated time
        /// </summary>
        public bool IsOverEstimate => TotalActualTimeMinutes > (EstimatedTimeHours * 60);

        /// <summary>
        /// Indicates if task is significantly over estimate (>20%)
        /// </summary>
        public bool IsSignificantlyOverEstimate => TotalActualTimeMinutes > (EstimatedTimeHours * 60 * 1.2m);

        /// <summary>
        /// CSS class for progress bar based on completion status
        /// </summary>
        public string ProgressBarClass
        {
            get
            {
                if (IsSignificantlyOverEstimate) return "bg-danger";
                if (IsOverEstimate) return "bg-warning";
                return "bg-success";
            }
        }

        /// <summary>
        /// Helper method to format time from hours to readable format
        /// </summary>
        private static string FormatTimeFromHours(decimal hours)
        {
            var totalMinutes = (int)(hours * 60);
            return FormatTimeFromMinutes(totalMinutes);
        }

        /// <summary>
        /// Helper method to format time from minutes to readable format (e.g., "2h 30m")
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
