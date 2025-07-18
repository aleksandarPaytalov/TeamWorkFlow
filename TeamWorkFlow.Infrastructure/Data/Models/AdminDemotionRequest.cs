using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using TeamWorkFlow.Infrastructure.Constants;

namespace TeamWorkFlow.Infrastructure.Data.Models
{
    /// <summary>
    /// Represents a pending request to demote an administrator that requires approval from another admin
    /// </summary>
    public class AdminDemotionRequest
    {
        /// <summary>
        /// Unique identifier for the demotion request
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID of the user to be demoted from admin role
        /// </summary>
        [Required]
        [MaxLength(DataConstants.OperatorUserIdMaxLength)]
        public string TargetUserId { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for the user to be demoted
        /// </summary>
        public IdentityUser TargetUser { get; set; } = null!;

        /// <summary>
        /// ID of the admin who requested the demotion
        /// </summary>
        [Required]
        [MaxLength(DataConstants.OperatorUserIdMaxLength)]
        public string RequestedByUserId { get; set; } = string.Empty;

        /// <summary>
        /// Navigation property for the admin who requested the demotion
        /// </summary>
        public IdentityUser RequestedByUser { get; set; } = null!;

        /// <summary>
        /// ID of the admin who approved the demotion (if approved)
        /// </summary>
        [MaxLength(DataConstants.OperatorUserIdMaxLength)]
        public string? ApprovedByUserId { get; set; }

        /// <summary>
        /// Navigation property for the admin who approved the demotion
        /// </summary>
        public IdentityUser? ApprovedByUser { get; set; }

        /// <summary>
        /// Reason for the demotion request
        /// </summary>
        [MaxLength(500)]
        public string? Reason { get; set; }

        /// <summary>
        /// When the demotion request was created
        /// </summary>
        [Required]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the demotion request was processed (approved/rejected)
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Current status of the demotion request
        /// </summary>
        [Required]
        public DemotionRequestStatus Status { get; set; } = DemotionRequestStatus.Pending;

        /// <summary>
        /// Comments from the approving admin
        /// </summary>
        [MaxLength(500)]
        public string? ApprovalComments { get; set; }

        /// <summary>
        /// When the request expires (auto-reject after certain time)
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7); // 7 days to approve

        /// <summary>
        /// Whether the request has expired
        /// </summary>
        public bool IsExpired => DateTime.UtcNow > ExpiresAt && Status == DemotionRequestStatus.Pending;

        /// <summary>
        /// Whether the request is still pending and not expired
        /// </summary>
        public bool IsPendingAndValid => Status == DemotionRequestStatus.Pending && !IsExpired;
    }

    /// <summary>
    /// Status of an admin demotion request
    /// </summary>
    public enum DemotionRequestStatus
    {
        /// <summary>
        /// Request is pending approval from another admin
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Request was approved and user was demoted
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Request was rejected by another admin
        /// </summary>
        Rejected = 2,

        /// <summary>
        /// Request was cancelled by the requesting admin
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Request expired without approval
        /// </summary>
        Expired = 4
    }
}
