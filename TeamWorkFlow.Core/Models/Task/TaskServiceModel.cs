using TeamWorkFlow.Core.Contracts;
using TeamWorkFlow.Core.Models.Pager;

namespace TeamWorkFlow.Core.Models.Task
{
    public class TaskServiceModel : ITaskModel
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
		public string Priority { get; set; } = string.Empty;
		public string? Deadline { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string? EndDate { get; set; }
        public int EstimatedTime { get; set; }
        public double? ActualTime { get; set; }
        public string? CompletedBy { get; set; }

        // Machine information
        public int? MachineId { get; set; }
        public string? MachineName { get; set; }
        public bool HasMachine => MachineId.HasValue;

        // Operator information
        public ICollection<TaskOperatorModel> Operators { get; set; } = new List<TaskOperatorModel>();
        public bool HasOperators => Operators.Any();

        // Enhanced card features - computed properties

        /// <summary>
        /// Progress percentage based on actual time vs estimated time
        /// </summary>
        public decimal ProgressPercentage
        {
            get
            {
                if (EstimatedTime <= 0) return 0;

                var actualTimeHours = ActualTime ?? 0;
                var progress = (decimal)(actualTimeHours / EstimatedTime) * 100;

                // Cap at 100% for display purposes
                return Math.Min(100, Math.Max(0, progress));
            }
        }

        /// <summary>
        /// Urgency level based on deadline proximity and task status
        /// </summary>
        public UrgencyLevel UrgencyLevel
        {
            get
            {
                if (string.IsNullOrEmpty(Deadline) || Status.ToLower() == "finished" || Status.ToLower() == "canceled")
                    return UrgencyLevel.None;

                if (!DateTime.TryParse(Deadline, out var deadlineDate))
                    return UrgencyLevel.None;

                var now = DateTime.Now;
                var daysUntilDeadline = (deadlineDate - now).TotalDays;

                // Overdue
                if (daysUntilDeadline < 0)
                    return UrgencyLevel.Overdue;

                // Critical - less than 1 day
                if (daysUntilDeadline <= 1)
                    return UrgencyLevel.Critical;

                // High - less than 3 days
                if (daysUntilDeadline <= 3)
                    return UrgencyLevel.High;

                // Medium - less than 7 days
                if (daysUntilDeadline <= 7)
                    return UrgencyLevel.Medium;

                // Low - more than 7 days
                return UrgencyLevel.Low;
            }
        }

        /// <summary>
        /// Days until deadline (negative if overdue)
        /// </summary>
        public int? DaysUntilDeadline
        {
            get
            {
                if (string.IsNullOrEmpty(Deadline) || !DateTime.TryParse(Deadline, out var deadlineDate))
                    return null;

                return (int)(deadlineDate - DateTime.Now).TotalDays;
            }
        }

        /// <summary>
        /// Indicates if the task is overdue
        /// </summary>
        public bool IsOverdue => UrgencyLevel == UrgencyLevel.Overdue;

        /// <summary>
        /// Indicates if the task is on track (actual time <= estimated time)
        /// </summary>
        public bool IsOnTrack
        {
            get
            {
                if (EstimatedTime <= 0) return true;
                var actualTimeHours = ActualTime ?? 0;
                return actualTimeHours <= EstimatedTime;
            }
        }

        /// <summary>
        /// Time variance in hours (positive = over estimate, negative = under estimate)
        /// </summary>
        public double TimeVarianceHours
        {
            get
            {
                var actualTimeHours = ActualTime ?? 0;
                return actualTimeHours - EstimatedTime;
            }
        }

        /// <summary>
        /// Time variance as percentage of estimated time
        /// </summary>
        public decimal TimeVariancePercentage
        {
            get
            {
                if (EstimatedTime <= 0) return 0;
                return (decimal)(TimeVarianceHours / EstimatedTime) * 100;
            }
        }

        /// <summary>
        /// Formatted display of estimated time
        /// </summary>
        public string EstimatedTimeFormatted => FormatTimeFromHours(EstimatedTime);

        /// <summary>
        /// Formatted display of actual time
        /// </summary>
        public string ActualTimeFormatted => FormatTimeFromHours(ActualTime ?? 0);

        /// <summary>
        /// Status color for visual indicators
        /// </summary>
        public string StatusColor => Status.ToLower() switch
        {
            "open" => "#6b7280",        // Gray
            "in progress" => "#3b82f6", // Blue
            "finished" => "#10b981",    // Green
            "canceled" => "#ef4444",    // Red
            _ => "#6b7280"              // Default gray
        };

        /// <summary>
        /// Priority color for visual indicators
        /// </summary>
        public string PriorityColor => Priority.ToLower() switch
        {
            "low" => "#10b981",      // Green
            "normal" => "#f59e0b",   // Yellow
            "high" => "#ef4444",     // Red
            _ => "#6b7280"           // Default gray
        };

        /// <summary>
        /// Urgency color for visual indicators
        /// </summary>
        public string UrgencyColor => UrgencyLevel switch
        {
            UrgencyLevel.Low => "#10b981",      // Green
            UrgencyLevel.Medium => "#f59e0b",   // Yellow
            UrgencyLevel.High => "#ef4444",     // Red
            UrgencyLevel.Critical => "#dc2626", // Dark red
            UrgencyLevel.Overdue => "#7f1d1d",  // Very dark red
            _ => "#6b7280"                      // Default gray
        };

        /// <summary>
        /// Progress color based on completion status
        /// </summary>
        public string ProgressColor
        {
            get
            {
                if (Status.ToLower() == "finished") return "#10b981"; // Green
                if (ProgressPercentage >= 100) return "#ef4444";      // Red (over estimate)
                if (ProgressPercentage >= 80) return "#f59e0b";       // Yellow (approaching limit)
                return "#3b82f6";                                      // Blue (on track)
            }
        }

        // Helper method for time formatting
        private static string FormatTimeFromHours(double hours)
        {
            if (hours == 0) return "0h";

            var totalMinutes = (int)(hours * 60);
            var h = totalMinutes / 60;
            var m = totalMinutes % 60;

            if (m == 0) return $"{h}h";
            return $"{h}h {m}m";
        }
    }

    public class TaskOperatorModel
    {
        public int OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
    }

    public class PaginatedTasksViewModel
    {
        public IEnumerable<TaskServiceModel> Tasks { get; set; } = new List<TaskServiceModel>();
        public PagerServiceModel Pager { get; set; } = null!;
    }

    /// <summary>
    /// Urgency levels for task deadline proximity
    /// </summary>
    public enum UrgencyLevel
    {
        None = 0,       // No deadline or task completed
        Low = 1,        // More than 7 days
        Medium = 2,     // 3-7 days
        High = 3,       // 1-3 days
        Critical = 4,   // Less than 1 day
        Overdue = 5     // Past deadline
    }

    /// <summary>
    /// Extension methods for enhanced task card functionality
    /// </summary>
    public static class TaskServiceModelExtensions
    {
        /// <summary>
        /// Calculate progress percentage for a collection of tasks
        /// </summary>
        public static decimal CalculateOverallProgress(this IEnumerable<TaskServiceModel> tasks)
        {
            var taskList = tasks.ToList();
            if (!taskList.Any()) return 0;

            var totalEstimated = taskList.Sum(t => t.EstimatedTime);
            if (totalEstimated <= 0) return 0;

            var totalActual = taskList.Sum(t => t.ActualTime ?? 0);
            return Math.Min(100, (decimal)(totalActual / totalEstimated) * 100);
        }

        /// <summary>
        /// Get tasks grouped by urgency level
        /// </summary>
        public static Dictionary<UrgencyLevel, List<TaskServiceModel>> GroupByUrgency(this IEnumerable<TaskServiceModel> tasks)
        {
            return tasks.GroupBy(t => t.UrgencyLevel)
                       .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Get tasks that are overdue
        /// </summary>
        public static IEnumerable<TaskServiceModel> GetOverdueTasks(this IEnumerable<TaskServiceModel> tasks)
        {
            return tasks.Where(t => t.IsOverdue);
        }

        /// <summary>
        /// Get tasks that are behind schedule (actual time > estimated time)
        /// </summary>
        public static IEnumerable<TaskServiceModel> GetBehindScheduleTasks(this IEnumerable<TaskServiceModel> tasks)
        {
            return tasks.Where(t => !t.IsOnTrack && (t.ActualTime ?? 0) > 0);
        }

        /// <summary>
        /// Calculate completion percentage based on finished tasks
        /// </summary>
        public static decimal CalculateCompletionPercentage(this IEnumerable<TaskServiceModel> tasks)
        {
            var taskList = tasks.ToList();
            if (!taskList.Any()) return 0;

            var finishedTasks = taskList.Count(t => t.Status.ToLower() == "finished");
            return (decimal)finishedTasks / taskList.Count * 100;
        }

        /// <summary>
        /// Get urgency display text
        /// </summary>
        public static string GetUrgencyDisplayText(this UrgencyLevel urgency)
        {
            return urgency switch
            {
                UrgencyLevel.None => "",
                UrgencyLevel.Low => "Low Priority",
                UrgencyLevel.Medium => "Medium Priority",
                UrgencyLevel.High => "High Priority",
                UrgencyLevel.Critical => "Critical",
                UrgencyLevel.Overdue => "Overdue",
                _ => ""
            };
        }

        /// <summary>
        /// Get urgency icon class
        /// </summary>
        public static string GetUrgencyIconClass(this UrgencyLevel urgency)
        {
            return urgency switch
            {
                UrgencyLevel.None => "",
                UrgencyLevel.Low => "fas fa-clock",
                UrgencyLevel.Medium => "fas fa-exclamation-triangle",
                UrgencyLevel.High => "fas fa-exclamation-circle",
                UrgencyLevel.Critical => "fas fa-fire",
                UrgencyLevel.Overdue => "fas fa-times-circle",
                _ => "fas fa-clock"
            };
        }

        /// <summary>
        /// Determine if task requires immediate attention
        /// </summary>
        public static bool RequiresImmediateAttention(this TaskServiceModel task)
        {
            return task.UrgencyLevel >= UrgencyLevel.Critical ||
                   (!task.IsOnTrack && task.UrgencyLevel >= UrgencyLevel.High);
        }

        /// <summary>
        /// Get formatted time variance display
        /// </summary>
        public static string GetTimeVarianceDisplay(this TaskServiceModel task)
        {
            var variance = task.TimeVarianceHours;
            if (Math.Abs(variance) < 0.1) return "On track";

            var sign = variance > 0 ? "+" : "";
            var hours = Math.Abs(variance);
            var formattedTime = FormatTimeFromHours(hours);

            return variance > 0 ? $"{sign}{formattedTime} over" : $"{formattedTime} under";
        }

        /// <summary>
        /// Helper method for formatting time from hours
        /// </summary>
        private static string FormatTimeFromHours(double hours)
        {
            if (hours == 0) return "0h";

            var totalMinutes = (int)(hours * 60);
            var h = totalMinutes / 60;
            var m = totalMinutes % 60;

            if (m == 0) return $"{h}h";
            return $"{h}h {m}m";
        }
    }
}
