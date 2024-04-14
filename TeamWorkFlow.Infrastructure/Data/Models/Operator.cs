using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
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

        [Required]
        [Comment("User identifier")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;
        
        public ICollection<TaskOperator> TasksOperators { get; set; } = new List<TaskOperator>();

        [Comment("Checker if the User is approved as operator")]
        public bool IsApproved { get; set; }
    }
}
