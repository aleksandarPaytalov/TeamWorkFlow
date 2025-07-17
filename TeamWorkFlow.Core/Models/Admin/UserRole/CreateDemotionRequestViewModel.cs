using System.ComponentModel.DataAnnotations;

namespace TeamWorkFlow.Core.Models.Admin.UserRole
{
    /// <summary>
    /// View model for creating a new admin demotion request
    /// </summary>
    public class CreateDemotionRequestViewModel
    {
        /// <summary>
        /// User ID of the admin to be demoted
        /// </summary>
        [Required]
        public string TargetUserId { get; set; } = string.Empty;

        /// <summary>
        /// Email of the admin to be demoted (for display)
        /// </summary>
        [Display(Name = "Admin to Demote")]
        public string TargetUserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the admin to be demoted (for display)
        /// </summary>
        [Display(Name = "Full Name")]
        public string TargetUserFullName { get; set; } = string.Empty;

        /// <summary>
        /// Reason for the demotion request
        /// </summary>
        [Required(ErrorMessage = "Please provide a reason for the demotion request.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Reason must be between 10 and 500 characters.")]
        [Display(Name = "Reason for Demotion")]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation that the user understands this requires another admin's approval
        /// </summary>
        [Display(Name = "I understand that this demotion request requires approval from another administrator")]
        public bool AcknowledgeApprovalRequired { get; set; }
    }
}
