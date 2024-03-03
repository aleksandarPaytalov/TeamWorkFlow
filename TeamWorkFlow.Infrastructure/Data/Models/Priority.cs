using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("Priority data model")]
    public class Priority
    {
        [Key]
        [Comment("Priority identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.PriorityNameMaxLength)]
        [Comment("Priority name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
