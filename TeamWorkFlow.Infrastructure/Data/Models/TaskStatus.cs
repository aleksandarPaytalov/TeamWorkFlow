using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("TaskStatus data model")]
    public class TaskStatus
    {
        [Key]
        [Comment("TaskStatus identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.TaskStatusNameMaxLength)]
        [Comment("TaskStatus name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
