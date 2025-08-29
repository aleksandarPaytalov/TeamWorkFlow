using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
	[Comment("Task Db model")]
    public class Task
    {
        [Key]
        [Comment("Task identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(TaskNameMaxLength)]
        [Comment("Task Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(TaskDescriptionMaxLength)]
        [Comment("Task description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Comment("Task starting date")]
        public DateTime StartDate { get; set; }

        [Comment("The date when the task is finished")]
        public DateTime? EndDate { get; set; }

        [Required]
        [ForeignKey(nameof(TaskStatus))]
        [Comment("TaskStatus identifier")]
        public int TaskStatusId { get; set; }

        [Required]
        public TaskStatus TaskStatus { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Priority))]
        [Comment("Priority identifier")]
        public int PriorityId { get; set; }

        [Required]
        public Priority Priority { get; set; } = null!;

        [Required]
        [MaxLength(TaskCreatorIdMaxLength)]
        [Comment("Task creator identifier")]
        public string CreatorId { get; set; } = string.Empty;

        [Required]
        [Comment("Task Creator")]
        public IdentityUser Creator { get; set; } = null!;

        public DateTime? DeadLine { get; set; }

        [Required]
        [Comment("Estimated time for the Task that is needed to be complete - in hours")]
        public int EstimatedTime { get; set; }

        [MaxLength(TaskCommentMaxLength)]
        [Comment("Comment for the current task")]
        public string? Comment { get; set; }

        [MaxLength(TaskAttachmentsMaxLength)]
        [Comment("Task attachments - files, drawings, documents, etc.")]
        public  string? Attachment { get; set; }

        [ForeignKey(nameof(Machine))]
        [Comment("Machine identifier")]
        public int? MachineId { get; set; }

        public Machine? Machine { get; set; }

        [Required]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public ICollection<TaskOperator> TasksOperators { get; set; } = new List<TaskOperator>();

        [Comment("Indicates if task is included in current sprint")]
        public bool IsInSprint { get; set; } = false;

        [Comment("Order position in sprint for drag-and-drop functionality")]
        public int SprintOrder { get; set; } = 0;

        [MaxLength(TaskCreatorIdMaxLength)]
        [Comment("User identifier who completed the task")]
        public string? CompletedById { get; set; }

        [ForeignKey(nameof(CompletedById))]
        [Comment("User who completed the task")]
        public IdentityUser? CompletedBy { get; set; }

        [Comment("Actual time spent on the task in hours (calculated from start and end dates)")]
        public double? ActualTime { get; set; }

        [Comment("Planned start date in sprint")]
        public DateTime? PlannedStartDate { get; set; }

        [Comment("Planned completion date in sprint")]
        public DateTime? PlannedEndDate { get; set; }
    }
}
