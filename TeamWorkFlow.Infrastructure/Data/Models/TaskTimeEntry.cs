using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("Task time entry data model - tracks individual completed work sessions")]
    public class TaskTimeEntry
    {
        [Key]
        [Comment("Task time entry identifier")]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Task))]
        [Comment("Task identifier")]
        public int TaskId { get; set; }

        [Required]
        [Comment("Task reference")]
        public Task Task { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Operator))]
        [Comment("Operator identifier")]
        public int OperatorId { get; set; }

        [Required]
        [Comment("Operator reference")]
        public Operator Operator { get; set; } = null!;

        [Required]
        [Comment("Work session start time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Comment("Work session end time")]
        public DateTime EndTime { get; set; }

        [Required]
        [Comment("Total duration of work session in minutes")]
        public int DurationMinutes { get; set; }

        [MaxLength(TaskTimeEntryNotesMaxLength)]
        [Comment("Optional notes about the work session")]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(TaskTimeEntrySessionTypeMaxLength)]
        [Comment("Type of work session (e.g., 'Development', 'Testing', 'Review')")]
        public string SessionType { get; set; } = string.Empty;

        [Required]
        [Comment("Timestamp when the time entry was created")]
        public DateTime CreatedAt { get; set; }
    }
}
