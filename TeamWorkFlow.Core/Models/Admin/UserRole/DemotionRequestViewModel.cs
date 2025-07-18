using System.ComponentModel.DataAnnotations;
using TeamWorkFlow.Infrastructure.Data.Models;

namespace TeamWorkFlow.Core.Models.Admin.UserRole
{
    /// <summary>
    /// View model for displaying admin demotion requests
    /// </summary>
    public class DemotionRequestViewModel
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID of the admin to be demoted
        /// </summary>
        public string TargetUserId { get; set; } = string.Empty;

        /// <summary>
        /// Email of the admin to be demoted
        /// </summary>
        [Display(Name = "Target Admin")]
        public string TargetUserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the admin to be demoted
        /// </summary>
        [Display(Name = "Full Name")]
        public string TargetUserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Email of the admin who requested the demotion
        /// </summary>
        [Display(Name = "Requested By")]
        public string RequestedByUserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the admin who requested the demotion
        /// </summary>
        [Display(Name = "Requested By")]
        public string RequestedByUserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Email of the admin who approved the demotion (if approved)
        /// </summary>
        [Display(Name = "Approved By")]
        public string? ApprovedByUserEmail { get; set; }

        /// <summary>
        /// Full name of the admin who approved the demotion (if approved)
        /// </summary>
        [Display(Name = "Approved By")]
        public string? ApprovedByUserFullName { get; set; }

        /// <summary>
        /// Reason for the demotion request
        /// </summary>
        [Display(Name = "Reason")]
        public string? Reason { get; set; }

        /// <summary>
        /// When the request was created
        /// </summary>
        [Display(Name = "Requested At")]
        public DateTime RequestedAt { get; set; }

        /// <summary>
        /// When the request was processed
        /// </summary>
        [Display(Name = "Processed At")]
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Current status of the request
        /// </summary>
        [Display(Name = "Status")]
        public DemotionRequestStatus Status { get; set; }

        /// <summary>
        /// Status display text
        /// </summary>
        [Display(Name = "Status")]
        public string StatusText => Status switch
        {
            DemotionRequestStatus.Pending => "Pending Approval",
            DemotionRequestStatus.Approved => "Approved",
            DemotionRequestStatus.Rejected => "Rejected",
            DemotionRequestStatus.Cancelled => "Cancelled",
            DemotionRequestStatus.Expired => "Expired",
            _ => "Unknown"
        };

        /// <summary>
        /// Comments from the approving admin
        /// </summary>
        [Display(Name = "Approval Comments")]
        public string? ApprovalComments { get; set; }

        /// <summary>
        /// When the request expires
        /// </summary>
        [Display(Name = "Expires At")]
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Whether the request has expired
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// Whether the request is still pending and valid
        /// </summary>
        public bool IsPendingAndValid { get; set; }

        /// <summary>
        /// Whether the current user can approve this request
        /// </summary>
        public bool CanApprove { get; set; }

        /// <summary>
        /// Whether the current user can reject this request
        /// </summary>
        public bool CanReject { get; set; }

        /// <summary>
        /// Whether the current user can cancel this request
        /// </summary>
        public bool CanCancel { get; set; }

        /// <summary>
        /// Time remaining until expiration
        /// </summary>
        public TimeSpan TimeUntilExpiration => ExpiresAt - DateTime.UtcNow;

        /// <summary>
        /// Human-readable time until expiration
        /// </summary>
        public string TimeUntilExpirationText
        {
            get
            {
                if (IsExpired) return "Expired";
                
                var timeLeft = TimeUntilExpiration;
                if (timeLeft.TotalDays >= 1)
                    return $"{(int)timeLeft.TotalDays} day(s)";
                if (timeLeft.TotalHours >= 1)
                    return $"{(int)timeLeft.TotalHours} hour(s)";
                if (timeLeft.TotalMinutes >= 1)
                    return $"{(int)timeLeft.TotalMinutes} minute(s)";
                
                return "Less than 1 minute";
            }
        }
    }
}
