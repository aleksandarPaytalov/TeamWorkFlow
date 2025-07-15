namespace TeamWorkFlow.Core.Models.Sprint
{
    public class SprintResourceServiceModel
    {
        public List<SprintOperatorModel> Operators { get; set; } = new List<SprintOperatorModel>();
        
        public List<SprintMachineModel> Machines { get; set; } = new List<SprintMachineModel>();
        
        public int TotalOperatorCapacity => Operators.Where(o => o.IsActive).Sum(o => o.WeeklyCapacity);
        
        public int TotalMachineCapacity => Machines.Where(m => m.IsActive).Sum(m => m.WeeklyCapacity);
        
        public int ActiveOperatorsCount => Operators.Count(o => o.IsActive);
        
        public int ActiveMachinesCount => Machines.Count(m => m.IsActive);
        
        public double AverageOperatorUtilization => Operators.Where(o => o.IsActive).Any() ? 
            Operators.Where(o => o.IsActive).Average(o => o.UtilizationPercentage) : 0;
        
        public double AverageMachineUtilization => Machines.Where(m => m.IsActive).Any() ? 
            Machines.Where(m => m.IsActive).Average(m => m.UtilizationPercentage) : 0;
    }
    
    public class SprintOperatorModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        
        public string AvailabilityStatus { get; set; } = string.Empty;
        
        public int WeeklyCapacity { get; set; } = 40; // Default 40 hours per week
        
        public int CurrentAssignedHours { get; set; } = 0;
        
        public double UtilizationPercentage => WeeklyCapacity > 0 ? 
            (double)CurrentAssignedHours / WeeklyCapacity * 100 : 0;
        
        public int AvailableHours => Math.Max(0, WeeklyCapacity - CurrentAssignedHours);
        
        public bool IsOverloaded => CurrentAssignedHours > WeeklyCapacity;
        
        public bool CanTakeMoreWork => IsActive && AvailableHours > 0 && AvailabilityStatus.ToLower() == "at work";
        
        public string GetUtilizationStatus()
        {
            if (!IsActive) return "Inactive";
            if (AvailabilityStatus.ToLower() != "at work") return "Unavailable";
            
            return UtilizationPercentage switch
            {
                >= 100 => "Overloaded",
                >= 90 => "At Capacity",
                >= 70 => "High Load",
                >= 40 => "Moderate Load",
                > 0 => "Light Load",
                _ => "Available"
            };
        }
        
        public string GetStatusColor()
        {
            return GetUtilizationStatus() switch
            {
                "Available" => "#10b981",      // Green
                "Light Load" => "#84cc16",     // Light Green
                "Moderate Load" => "#f59e0b",  // Yellow
                "High Load" => "#f97316",      // Orange
                "At Capacity" => "#ef4444",   // Red
                "Overloaded" => "#dc2626",     // Dark Red
                "Unavailable" => "#6b7280",   // Gray
                "Inactive" => "#374151",      // Dark Gray
                _ => "#6b7280"
            };
        }
    }
    
    public class SprintMachineModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        
        public int WeeklyCapacity { get; set; } = 168; // Default 24/7 operation (168 hours per week)
        
        public int CurrentAssignedHours { get; set; } = 0;
        
        public double UtilizationPercentage => WeeklyCapacity > 0 ? 
            (double)CurrentAssignedHours / WeeklyCapacity * 100 : 0;
        
        public int AvailableHours => Math.Max(0, WeeklyCapacity - CurrentAssignedHours);
        
        public bool IsOverloaded => CurrentAssignedHours > WeeklyCapacity;
        
        public bool CanTakeMoreWork => IsActive && AvailableHours > 0;
        
        public string GetUtilizationStatus()
        {
            if (!IsActive) return "Inactive";
            
            return UtilizationPercentage switch
            {
                >= 100 => "Overloaded",
                >= 90 => "At Capacity",
                >= 70 => "High Load",
                >= 40 => "Moderate Load",
                > 0 => "Light Load",
                _ => "Available"
            };
        }
        
        public string GetStatusColor()
        {
            return GetUtilizationStatus() switch
            {
                "Available" => "#10b981",      // Green
                "Light Load" => "#84cc16",     // Light Green
                "Moderate Load" => "#f59e0b",  // Yellow
                "High Load" => "#f97316",      // Orange
                "At Capacity" => "#ef4444",   // Red
                "Overloaded" => "#dc2626",     // Dark Red
                "Inactive" => "#374151",      // Dark Gray
                _ => "#6b7280"
            };
        }
    }
}
