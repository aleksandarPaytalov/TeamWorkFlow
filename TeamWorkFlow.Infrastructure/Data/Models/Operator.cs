using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static TeamWorkFlow.Infrastructure.Constants.DataConstants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
	[Comment("Operator DB model")]
    public class Operator
    {
        [Key]
        [Comment("Operator identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(OperatorFullNameMaxLength)]
        [Comment("First and Last name of the operator")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(AvailabilityStatus))]
        [Comment("Operator status identifier")]
        public int AvailabilityStatusId { get; set; }

        [Required]
        [Comment("Availability status of the operator")]
        public OperatorAvailabilityStatus AvailabilityStatus { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(OperatorEmailMaxLength)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(OperatorPhoneMaxLength)]
        [Comment("Operator phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Comment("Showing if the current operator is still working in the company")]
        public bool IsActive { get; set; }

        [Required]
        [Range(OperatorMinCapacity, OperatorMaxCapacity)]
        [Comment("Operator working capacity in hours per day/shift")]
        public int Capacity { get; set; }
        
        public ICollection<TaskOperator> TasksOperators { get; set; } = new List<TaskOperator>();
    }
}
