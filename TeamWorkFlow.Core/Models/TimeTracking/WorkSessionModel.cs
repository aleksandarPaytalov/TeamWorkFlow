using System;
using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.TimeTracking
{
    /// <summary>
    /// View model for individual work session data
    /// </summary>
    public class WorkSessionModel
    {
        /// <summary>
        /// Session identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Task identifier this session belongs to
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Task name for display purposes
        /// </summary>
        [Display(Name = "Task")]
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Operator identifier who worked on this session
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// Operator name for display purposes
        /// </summary>
        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        /// <summary>
        /// Session start time
        /// </summary>
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Session end time (for completed sessions)
        /// </summary>
        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Total duration of the session in minutes
        /// </summary>
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Session notes or comments
        /// </summary>
        [Display(Name = "Notes")]
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Type of session (e.g., "Development", "Testing", "Review")
        /// </summary>
        [Display(Name = "Session Type")]
        public string SessionType { get; set; } = string.Empty;

        /// <summary>
        /// When this session record was created
        /// </summary>
        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indicates if this is an active session (in progress)
        /// </summary>
        public bool IsActive => !EndTime.HasValue;

        /// <summary>
        /// Formatted display of session duration (e.g., "2h 30m")
        /// </summary>
        [Display(Name = "Duration")]
        public string DurationFormatted
        {
            get
            {
                if (DurationMinutes <= 0) return "0m";

                var hours = DurationMinutes / 60;
                var minutes = DurationMinutes % 60;

                if (hours > 0 && minutes > 0)
                    return $"{hours}h {minutes}m";
                else if (hours > 0)
                    return $"{hours}h";
                else
                    return $"{minutes}m";
            }
        }

        /// <summary>
        /// Formatted display of start time
        /// </summary>
        [Display(Name = "Started")]
        public string StartTimeFormatted => StartTime.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// Formatted display of end time
        /// </summary>
        [Display(Name = "Finished")]
        public string EndTimeFormatted => EndTime?.ToString("dd/MM/yyyy HH:mm") ?? "In Progress";

        /// <summary>
        /// Formatted display of creation date
        /// </summary>
        [Display(Name = "Created")]
        public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// CSS class for session status display
        /// </summary>
        public string StatusClass
        {
            get
            {
                if (IsActive) return "badge bg-primary";
                return "badge bg-success";
            }
        }

        /// <summary>
        /// Display text for session status
        /// </summary>
        public string StatusText
        {
            get
            {
                if (IsActive) return "In Progress";
                return "Completed";
            }
        }

        /// <summary>
        /// CSS class for duration display based on length
        /// </summary>
        public string DurationClass
        {
            get
            {
                if (DurationMinutes >= 480) return "text-success fw-bold"; // 8+ hours
                if (DurationMinutes >= 240) return "text-warning"; // 4+ hours
                if (DurationMinutes >= 60) return "text-info"; // 1+ hour
                return "text-muted"; // Less than 1 hour
            }
        }

        /// <summary>
        /// Indicates if session has notes
        /// </summary>
        public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

        /// <summary>
        /// Truncated notes for display in lists (max 50 characters)
        /// </summary>
        public string NotesPreview
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Notes)) return string.Empty;
                return Notes.Length > 50 ? Notes.Substring(0, 47) + "..." : Notes;
            }
        }
    }
}
