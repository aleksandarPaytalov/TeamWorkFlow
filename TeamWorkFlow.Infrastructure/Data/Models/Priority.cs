using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;


namespace TeamWorkFlow.Infrastructure.Data.Models
{
	[Comment("Priority data model")]
    public class Priority
    {
        [Key]
        [Comment("Priority identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(PriorityNameMaxLength)]
        [Comment("Priority name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
