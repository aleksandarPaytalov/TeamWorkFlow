using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("Operator availability status db model")]
    public class AvailabilityStatus
    {
        [Key]
        [Comment("Operator identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.AvailabilityStatusNameMaxLength)]
        [Comment("Availability status name")]
        public string Name { get; set; } = string.Empty;

        public ICollection<Operator> Operators { get; set; } = new List<Operator>();
    }
}
