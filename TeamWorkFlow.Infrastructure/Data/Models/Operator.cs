﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    [Comment("Operator DB model")]
    public class Operator
    {
        [Key]
        [Comment("Operator identifier")]
        public int Id { get; set; }

        [Required]
        [MaxLength(DataConstants.OperatorFullNameMaxLength)]
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
        [MaxLength(DataConstants.OperatorEmailMaxLength)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(DataConstants.OperatorPhoneMaxLength)]
        [Comment("Operator phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Comment("Showing if the current operator is still working in the company")]
        public bool IsActive { get; set; }

        [Required]
        [Comment("Operator working capacity in hours per day/shift")]
        public int Capacity { get; set; }
        
        public ICollection<TaskOperator> TasksOperators { get; set; } = new List<TaskOperator>();
    }
}
