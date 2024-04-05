using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
	[Comment("Part Db model")]
    public class Part
    {
        [Key]
        [Comment("Part identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(PartNameMaxLength)]
        [Comment("Part name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(PartArticleNumberMaxLength)]
        [Comment("Part article number")]
        public string PartArticleNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(PartClientNumberMaxLength)]
        [Comment("Client article number for the current part")]
        public string PartClientNumber { get; set; } = string.Empty;

        [Required]
        [Range(PartToolNumberMinValue, 
            PartToolNumberMaxValue)]
        [Comment("Part tool number")]
        public int ToolNumber { get; set; }

        [Required]
        [ForeignKey(nameof(PartStatus))]
        [Comment("PartStatus identifier")]
        public int PartStatusId { get; set; }
        public PartStatus PartStatus { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        [Required]
        public Project Project { get; set; } = null!;

        [Required]
        [MaxLength(PartImageUrlMaxLength)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(PartModelMaxLength)]
        public string PartModel { get; set; } = string.Empty;
    }
}
