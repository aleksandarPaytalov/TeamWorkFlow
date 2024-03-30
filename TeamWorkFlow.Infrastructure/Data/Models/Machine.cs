using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("Machine db model")]
    public class Machine
    {
        [Key]
        [Comment("Machine identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.MachineNameMaxLength)]
        [Comment("Machine name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Comment("Machine capacity")]
        public int Capacity { get; set; }

        [Comment("Machine maintenanceScheduleStartDate")]
        public DateTime? MaintenanceScheduleStartDate { get; set; }

        [Comment("Machine maintenanceScheduleEndDate")]
        public DateTime? MaintenanceScheduleEndDate { get; set; }

        [Required]
        [Comment("Machine calibration schedule")]
        public DateTime CalibrationSchedule { get; set; }

        [Required]
        [Comment("Machine total load")]
        public double TotalMachineLoad { get; set; }

        [Required]
        public bool IsCalibrated { get; set; }

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
