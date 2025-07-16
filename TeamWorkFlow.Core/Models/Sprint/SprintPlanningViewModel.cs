namespace TeamWorkFlow.Core.Models.Sprint
{
    public class SprintPlanningViewModel
    {
        public List<SprintTaskServiceModel> SprintTasks { get; set; } = new List<SprintTaskServiceModel>();
        
        public List<SprintTaskServiceModel> BacklogTasks { get; set; } = new List<SprintTaskServiceModel>();
        
        public SprintCapacityServiceModel Capacity { get; set; } = new SprintCapacityServiceModel();
        
        public SprintResourceServiceModel Resources { get; set; } = new SprintResourceServiceModel();
        
        public SprintSummaryModel Summary { get; set; } = new SprintSummaryModel();
        
        public List<SprintTimelineModel> Timeline { get; set; } = new List<SprintTimelineModel>();
        
        // Filter and search properties
        public string SearchTerm { get; set; } = string.Empty;
        
        public string StatusFilter { get; set; } = string.Empty;
        
        public string PriorityFilter { get; set; } = string.Empty;
        
        public string ProjectFilter { get; set; } = string.Empty;
        
        public string OperatorFilter { get; set; } = string.Empty;
        
        public string MachineFilter { get; set; } = string.Empty;
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        
        public int PageSize { get; set; } = 20;
        
        public int TotalTasks { get; set; }
        
        public int TotalPages => (int)Math.Ceiling((double)TotalTasks / PageSize);
        
        // Sprint configuration
        public DateTime SprintStartDate { get; set; } = DateTime.Today;
        
        public DateTime SprintEndDate { get; set; } = DateTime.Today.AddDays(14); // Default 2-week sprint
        
        public int SprintDurationDays => (SprintEndDate - SprintStartDate).Days + 1;
        
        public int SprintWorkingHours => SprintDurationDays * 8; // Assuming 8 hours per day
        
        // Drag and drop support
        public bool IsDragDropEnabled { get; set; } = true;
        
        public string LastUpdatedBy { get; set; } = string.Empty;
        
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
    
    public class SprintSummaryModel
    {
        public int TotalTasksInSprint { get; set; }
        
        public int CompletedTasks { get; set; }
        
        public int InProgressTasks { get; set; }
        
        public int NotStartedTasks { get; set; }
        
        public int OnHoldTasks { get; set; }
        
        public int TotalEstimatedHours { get; set; }
        
        public decimal TotalActualHours { get; set; }
        
        public int HighPriorityTasks { get; set; }
        
        public int CriticalPriorityTasks { get; set; }
        
        public int OverdueTasks { get; set; }
        
        public double CompletionPercentage => TotalTasksInSprint > 0 ? 
            (double)CompletedTasks / TotalTasksInSprint * 100 : 0;
        
        public double ProgressPercentage => TotalEstimatedHours > 0 ? 
            (double)TotalActualHours / TotalEstimatedHours * 100 : 0;
        
        public bool IsOnTrack => CompletionPercentage >= 80 && OverdueTasks == 0;
        
        public string GetSprintHealth()
        {
            if (OverdueTasks > 0) return "At Risk";
            if (CompletionPercentage >= 90) return "Excellent";
            if (CompletionPercentage >= 70) return "Good";
            if (CompletionPercentage >= 50) return "Fair";
            return "Poor";
        }
        
        public string GetHealthColor()
        {
            return GetSprintHealth() switch
            {
                "Excellent" => "#10b981", // Green
                "Good" => "#84cc16",      // Light Green
                "Fair" => "#f59e0b",      // Yellow
                "Poor" => "#ef4444",      // Red
                "At Risk" => "#dc2626",   // Dark Red
                _ => "#6b7280"            // Gray
            };
        }
    }
    
    public class SprintTimelineModel
    {
        public DateTime Date { get; set; }
        
        public List<SprintTaskServiceModel> TasksStarting { get; set; } = new List<SprintTaskServiceModel>();
        
        public List<SprintTaskServiceModel> TasksEnding { get; set; } = new List<SprintTaskServiceModel>();
        
        public List<SprintTaskServiceModel> TasksInProgress { get; set; } = new List<SprintTaskServiceModel>();
        
        public int TotalHoursScheduled { get; set; }
        
        public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;
        
        public bool IsToday => Date.Date == DateTime.Today;
        
        public bool IsOverloaded { get; set; }
        
        public string GetDayStatus()
        {
            if (IsWeekend) return "Weekend";
            if (IsOverloaded) return "Overloaded";
            if (TotalHoursScheduled > 6) return "Busy";
            if (TotalHoursScheduled > 3) return "Moderate";
            if (TotalHoursScheduled > 0) return "Light";
            return "Free";
        }
        
        public string GetStatusColor()
        {
            return GetDayStatus() switch
            {
                "Free" => "#10b981",      // Green
                "Light" => "#84cc16",     // Light Green
                "Moderate" => "#f59e0b",  // Yellow
                "Busy" => "#f97316",      // Orange
                "Overloaded" => "#ef4444", // Red
                "Weekend" => "#6b7280",   // Gray
                _ => "#6b7280"
            };
        }
    }
}
