using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("Task time session data model - tracks active work sessions in progress")]
    public class TaskTimeSession
    {
        [Key]
        [Comment("Task time session identifier")]
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

        [Comment("Last time the session was paused")]
        public DateTime? LastPauseTime { get; set; }

        [Required]
        [Comment("Total minutes the session has been paused")]
        public int TotalPausedMinutes { get; set; } = 0;

        [Required]
        [Comment("Indicates if the session is currently paused")]
        public bool IsPaused { get; set; } = false;

        [Required]
        [MaxLength(TaskTimeSessionStatusMaxLength)]
        [Comment("Current status of the session (e.g., 'Active', 'Paused', 'Completed')")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [Comment("Timestamp when the session was created")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Comment("Timestamp when the session was last updated")]
        public DateTime UpdatedAt { get; set; }
    }
}
