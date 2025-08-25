using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Sprint
{
    public class SprintTaskServiceModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public DateTime? DeadLine { get; set; }
        
        public int EstimatedTime { get; set; }
        
        public string TaskStatus { get; set; } = string.Empty;
        
        public string Priority { get; set; } = string.Empty;
        
        public string ProjectName { get; set; } = string.Empty;
        
        public string ProjectNumber { get; set; } = string.Empty;
        
        public string? MachineName { get; set; }
        
        public List<string> AssignedOperators { get; set; } = new List<string>();
        
        public int SprintOrder { get; set; } = 0;
        
        public bool IsInSprint { get; set; } = false;
        
        public DateTime? PlannedStartDate { get; set; }
        
        public DateTime? PlannedEndDate { get; set; }
        
        public decimal ActualHours { get; set; } = 0;
        
        public string CreatorName { get; set; } = string.Empty;
        
        public string? Comment { get; set; }
        
        public string? Attachment { get; set; }
        
        // Calculated properties for capacity planning
        public int RequiredOperatorHours => EstimatedTime;
        
        public int RequiredMachineHours => MachineId.HasValue ? EstimatedTime : 0;
        
        public int? MachineId { get; set; }
        
        public bool CanBeCompleted { get; set; } = true;
        
        public string StatusReason { get; set; } = string.Empty;
        
        // GitHub RoadMap style properties
        public string StatusColor => TaskStatus.ToLower() switch
        {
            "not started" => "#6b7280", // Gray
            "in progress" => "#3b82f6", // Blue
            "finished" => "#10b981",    // Green
            "on hold" => "#f59e0b",     // Yellow
            _ => "#6b7280"
        };
        
        public string PriorityColor => Priority.ToLower() switch
        {
            "low" => "#10b981",      // Green
            "medium" => "#f59e0b",   // Yellow
            "high" => "#ef4444",     // Red
            "critical" => "#dc2626", // Dark Red
            _ => "#6b7280"
        };
        
        public string GetFormattedDuration()
        {
            if (EstimatedTime < 8)
                return $"{EstimatedTime}h";
            
            int days = EstimatedTime / 8;
            int hours = EstimatedTime % 8;
            
            if (hours == 0)
                return $"{days}d";
            
            return $"{days}d {hours}h";
        }
        
        public string GetTimelineStatus()
        {
            // If task is finished, always show "Done" regardless of timeline
            if (TaskStatus.ToLower() == "finished")
                return "Done";

            if (!PlannedStartDate.HasValue || !PlannedEndDate.HasValue)
                return "Not Scheduled";

            var now = DateTime.Now;

            if (now < PlannedStartDate)
                return "Upcoming";
            else if (now >= PlannedStartDate && now <= PlannedEndDate)
                return "In Progress";
            else if (now > PlannedEndDate)
                return "Overdue";
            else
                return "Completed";
        }
    }
}
