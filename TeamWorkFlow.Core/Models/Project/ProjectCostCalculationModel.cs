using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Core.Constants.Messages;

namespace TeamWorkFlow.Core.Models.Project
{
    public class ProjectCostCalculationModel
    {
        public int ProjectId { get; set; }
        
        public string ProjectName { get; set; } = string.Empty;
        
        public string ProjectNumber { get; set; } = string.Empty;
        
        public double CalculatedActualHours { get; set; }

        [Required(ErrorMessage = RequiredMessage)]
        [Range(0.01, 10000.00, ErrorMessage = "Hourly rate must be between 0.01 and 10,000.00")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        public decimal TotalLaborCost => (decimal)CalculatedActualHours * HourlyRate;

        public string FormattedCalculatedActualHours => GetFormattedDuration(CalculatedActualHours);
        
        private string GetFormattedDuration(double hours)
        {
            if (hours < 0.1) return "0h";

            var days = (int)(hours / 24);
            var remainingHours = hours % 24;

            if (days > 0)
            {
                return remainingHours > 0.1 ? $"{days}d {remainingHours:F1}h" : $"{days}d";
            }

            return $"{remainingHours:F1}h";
        }
    }
}
