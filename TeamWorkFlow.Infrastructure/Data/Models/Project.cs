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
    }
}
