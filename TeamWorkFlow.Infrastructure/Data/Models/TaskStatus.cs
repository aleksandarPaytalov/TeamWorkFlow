using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
	[Comment("TaskStatus data model")]
    public class TaskStatus
    {
        [Key]
        [Comment("TaskStatus identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(TaskStatusNameMaxLength)]
        [Comment("TaskStatus name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
