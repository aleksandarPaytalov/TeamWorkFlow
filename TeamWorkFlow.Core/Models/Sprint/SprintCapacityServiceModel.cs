namespace TeamWorkFlow.Core.Models.Sprint
{
    public class SprintCapacityServiceModel
    {
        public int TotalOperatorHours { get; set; }
        
        public int TotalMachineHours { get; set; }
        
        public int RequiredOperatorHours { get; set; }
        
        public int RequiredMachineHours { get; set; }
        
        public int AvailableOperators { get; set; }
        
        public int AvailableMachines { get; set; }
        
        public List<OperatorCapacityModel> OperatorCapacities { get; set; } = new List<OperatorCapacityModel>();
        
        public List<MachineCapacityModel> MachineCapacities { get; set; } = new List<MachineCapacityModel>();
        
        public bool CanCompleteAllTasks => 
            RequiredOperatorHours <= TotalOperatorHours && 
            RequiredMachineHours <= TotalMachineHours;
        
        public int OperatorHoursDeficit => Math.Max(0, RequiredOperatorHours - TotalOperatorHours);
        
        public int MachineHoursDeficit => Math.Max(0, RequiredMachineHours - TotalMachineHours);
        
        public double OperatorUtilization => TotalOperatorHours > 0 ? 
            (double)RequiredOperatorHours / TotalOperatorHours * 100 : 0;
        
        public double MachineUtilization => TotalMachineHours > 0 ? 
            (double)RequiredMachineHours / TotalMachineHours * 100 : 0;
        
        public string GetCapacityStatus()
        {
            if (CanCompleteAllTasks)
            {
                if (OperatorUtilization > 90 || MachineUtilization > 90)
                    return "At Capacity";
                else if (OperatorUtilization > 70 || MachineUtilization > 70)
                    return "High Utilization";
                else
                    return "Available Capacity";
            }
            else
            {
                return "Over Capacity";
            }
        }
        
        public string GetStatusColor()
        {
            return GetCapacityStatus() switch
            {
                "Available Capacity" => "#10b981", // Green
                "High Utilization" => "#f59e0b",   // Yellow
                "At Capacity" => "#ef4444",        // Red
                "Over Capacity" => "#dc2626",      // Dark Red
                _ => "#6b7280"                      // Gray
            };
        }
    }
    
    public class OperatorCapacityModel
    {
        public int Id { get; set; }
        
        public string FullName { get; set; } = string.Empty;
        
        public int AvailableHours { get; set; }
        
        public int AssignedHours { get; set; }
        
        public bool IsActive { get; set; }
        
        public string AvailabilityStatus { get; set; } = string.Empty;
        
        public double UtilizationPercentage => AvailableHours > 0 ? 
            (double)AssignedHours / AvailableHours * 100 : 0;
        
        public int RemainingHours => Math.Max(0, AvailableHours - AssignedHours);
        
        public bool IsOverloaded => AssignedHours > AvailableHours;
    }
    
    public class MachineCapacityModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public int AvailableHours { get; set; }
        
        public int AssignedHours { get; set; }
        
        public bool IsActive { get; set; }
        
        public double UtilizationPercentage => AvailableHours > 0 ? 
            (double)AssignedHours / AvailableHours * 100 : 0;
        
        public int RemainingHours => Math.Max(0, AvailableHours - AssignedHours);
        
        public bool IsOverloaded => AssignedHours > AvailableHours;
    }
}
