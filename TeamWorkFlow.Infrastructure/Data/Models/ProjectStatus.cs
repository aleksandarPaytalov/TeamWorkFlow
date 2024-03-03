using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("ProjectStatus data model")]
    public class ProjectStatus
    {
        [Key]
        [Comment("ProjectStatus identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.ProjectStatusNameMaxLength)]
        [Comment("ProjectStatus name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Project> Tasks { get; set; } = new List<Project>();
    }
}
