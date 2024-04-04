using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
	[Comment("ProjectStatus data model")]
    public class ProjectStatus
    {
        [Key]
        [Comment("ProjectStatus identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(ProjectStatusNameMaxLength)]
        [Comment("ProjectStatus name")]
        public string Name { get; set; } = string.Empty;
        
        public ICollection<Project> Tasks { get; set; } = new List<Project>();
    }
}
