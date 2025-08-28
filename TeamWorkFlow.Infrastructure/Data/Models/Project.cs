using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("Project data model")]
    public class Project
    {
        [Key]
        [Comment("Project identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(ProjectNumberMaxLength)]
        [Comment("Project number")]
        public string ProjectNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(ProjectNameMaxLength)]
        [Comment("Project name")]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(ProjectStatus))]
        [Comment("ProjectStatus identifier")]
        public int ProjectStatusId { get; set; }

        [Required]
        public ProjectStatus ProjectStatus { get; set; } = null!;

        [MaxLength(ProjectClientNameMaxLength)]
        [Comment("Client name")]
        public string? ClientName { get; set; }

        [MaxLength(ProjectApplianceMaxLength)]
        [Comment("Project appliance sector")]
        public string? Appliance { get; set; }

        [Required]
        public int TotalHoursSpent { get; set; }

        public ICollection<Part> Parts { get; set; } = new List<Part>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();

        // Calculated properties for task-based time tracking
        /// <summary>
        /// Total hours from all finished tasks (TaskStatusId = 3)
        /// </summary>
        public int CalculatedTotalHours => Tasks
            .Where(t => t.TaskStatusId == 3) // 3 = "finished" status
            .Sum(t => t.EstimatedTime);

        /// <summary>
        /// Total estimated hours from all tasks regardless of status
        /// </summary>
        public int TotalEstimatedHours => Tasks.Sum(t => t.EstimatedTime);

        /// <summary>
        /// Variance between manual estimate (TotalHoursSpent) and actual finished task hours
        /// Positive value means manual estimate is higher than actual
        /// Negative value means actual work exceeded manual estimate
        /// </summary>
        public int TimeVariance => TotalHoursSpent - CalculatedTotalHours;

        /// <summary>
        /// Percentage of project completion based on finished tasks
        /// </summary>
        public double CompletionPercentage => TotalEstimatedHours > 0
            ? (double)CalculatedTotalHours / TotalEstimatedHours * 100
            : 0;

        /// <summary>
        /// Number of finished tasks
        /// </summary>
        public int FinishedTasksCount => Tasks.Count(t => t.TaskStatusId == 3);

        /// <summary>
        /// Number of tasks in progress
        /// </summary>
        public int InProgressTasksCount => Tasks.Count(t => t.TaskStatusId == 2);

        /// <summary>
        /// Number of open tasks
        /// </summary>
        public int OpenTasksCount => Tasks.Count(t => t.TaskStatusId == 1);
    }
}
