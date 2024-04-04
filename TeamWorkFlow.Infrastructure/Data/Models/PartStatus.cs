using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
	[Comment("Part status Db model")]
    public class PartStatus
    {
        [Key]
        [Comment("PartStatus identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(PartStatusNameMaxLength)]
        [Comment("PartStatus name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Part> Parts { get; set; } = new List<Part>();
    }
}
